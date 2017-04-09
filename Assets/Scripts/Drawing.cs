using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implementa o desenho sobre a folha de papel.
/// </summary>
public class Drawing: MonoBehaviour
{
    public Texture baseTexture;

    /// <summary>
    /// Textura dinâmica (render texture) usada com a render câmera para simular o desenho.
    /// </summary>
    public RenderTexture dynamicTexture;

    /// <summary>
    /// Material-base usado para a área na frente da câmera de renderização (e atrás das máscaras de desenho criadas).
    /// </summary>
    public Material baseMaterial;

    /// <summary>
    /// Sprite com o cursor para identificação de onde será desenhado.
    /// Só é apresentado quando executando no editor ou em outra plataforma
    /// que não seja Android ou iOS.
    /// </summary>
    public GameObject cursor;

    /// <summary>
    /// Objeto contenedor dos brushes criados conforme o desenho é realizado.
    /// </summary>
    public GameObject brushContainer;

    /// <summary>
    /// Posição-base da textura, para localização dos brushes de desenho e do cursor.
    /// </summary>
    private Vector3 basePos;

    /// <summary>
    /// Indicação lógica sobre a aplicação estar sendo executada em um ambiente móvel
    /// (isto é, com touch) ou não.
    /// </summary>
    private bool isMobile;

    /// <summary>
    /// Indicação sobre estar ou não desenhando na área de renderização.
    /// </summary>
    private bool isDrawing;

    /// <summary>
    /// Máscaras utilizadas para desenho dos brushes com a ferramenta lápiz.
    /// </summary>
    private Sprite[] masksPencil;

    /// <summary>
    /// Máscaras utilizadas para desenho dos brushes com a ferramenta pincel.
    /// </summary>
    private Sprite[] masksPaintbrush;

    /// <summary>
    /// Salva o valor da última coordenada em Z de uma nova máscara adicionada.
    /// Isto é usado para garantir que um novo desenho esteja sempre à frente de todos os
    /// demais já realizados.
    /// </summary>
    private float lastZ = 0.9999f;

    /// <summary>
    /// Valor de decremento no eixo Z para uma nova máscara adicionada.
    /// Isto é usado para garantir que um novo desenho esteja sempre à frente de todos os
    /// demais já realizados;
    /// </summary>
    private float zStep = -0.0001f;

    /// <summary>
    /// Número máximo de máscaras que são adicionadas durante o desenho antes de que a textura
    /// seja "baked" (as máscaras são todas unidas em uma só textura). Como esse processamento
    /// é computacionalmente custoso, ele não é realizado a cada desenho; mas precisa ser feito
    /// com regularidade para evitar que o número de objetos instanciados cresça absurdamente
    /// (o que deixaria a execução progressivamente mais lenta).
    /// </summary>
    private int maxMasks = 1000;

    /// <summary>
    /// Flag indicativo da realização do "baking" de textura das máscaras (enquanto o baking
    /// ocorre, o desenho não é realizado).
    /// </summary>
    private bool isBaking;

    /// <summary>
    /// Inicialização da classe.
    /// </summary>
    void Start()
    {
        // Garante que a área desenhável sempre começa com a textura base (totalmente branca)
        baseMaterial.mainTexture = baseTexture;

        basePos = brushContainer.transform.position;
        isDrawing = false;
        isBaking = false;
        cursor.SetActive(false);

        // Carrega as máscaras para as duas ferramentas
        masksPencil = Resources.LoadAll<Sprite>(@"masks-pencil");
        masksPaintbrush = Resources.LoadAll<Sprite>(@"masks-paintbrush");

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        isMobile = true;        
#else
        isMobile = false;
#endif
    }

	/// <summary>
    /// Captura o evento de desabilitação deste script. Quando isso ocorre,
    /// desliga o modo de desenho se ele estiver habilitado.
    /// </summary>
    void OnDisable()
    {
        isDrawing = false;
    }

	/// <summary>
    /// Atualização a cada quadro do jogo. Implementa toda a lógica de desenho e exibição
    /// do cursor.
    /// </summary>
	void Update()
    {
        // Verifica o início e o fim do desenho
        bool down = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0);
        bool up = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0);

        if (!isDrawing && down)
            isDrawing = true;
        else if (isDrawing && up)
            isDrawing = false;

        // Verifica se o mouse/dedo está sobre a área desenhável
        Vector3 pos;
        if(!isBaking && HitTestDrawableArea(out pos))
        {
            // Exibe o cursor (apenas em plataformas não móveis)
            if(!isMobile)
            {
                cursor.SetActive(true);
                Vector3 curPos = pos + basePos;
                curPos.z = -6.5f;
                cursor.transform.position = curPos;
            }

            // Desenha!
            if(isDrawing)
                draw(pos);
        }
        else
            cursor.SetActive(false);
    }

    /// <summary>
    /// Desenha uma nova "pincelada" com a ferramenta, brush e cor atualmente ativos na textura
    /// na posição dada. Na prática uma nova instância de uma das máscaras é adicionada em frente
    /// à câmera utilizada para implementar a textura dinâmica.
    /// </summary>
    /// <param name="pos">Posição da cena onde adicionar a nova "pincelada".</param>
    private void draw(Vector3 pos)
    {
        // Obtém o sprite e a cor selecionados para a máscara
        Sprite sprite;
        Color color;
        if (!buildMask(out sprite, out color))
            return;

        pos.z = lastZ + zStep;

        // Cria o objeto da máscara e adiciona-o em frente à textura dinâmica
        GameObject mask = new GameObject();

        mask.transform.parent = brushContainer.transform;
        mask.transform.localPosition = pos;
        mask.transform.localScale = Vector3.one * 0.05f;

        // Define o sprite e a cor conforme selecionado pelo usuário
        SpriteRenderer rend = mask.AddComponent<SpriteRenderer>();
        rend.sprite = sprite;
        rend.color = color;

        lastZ = pos.z;

        // Verifica se chegou o momento de fazer o "bake" da textura;
        // Se chegou, une todas as máscaras existentes em uma só e deleta os demais objetos
        if(brushContainer.transform.childCount > maxMasks)
        {
            // Esconde o cursor (para que ele não seja incluído na textura produzida)
            cursor.SetActive(false);

            // Indica que o "bake" está em progresso e chama-o assincronamente (em 100 milisegundos)
            isBaking = true;
            Invoke("bakeMasksTexture", 0.1f);
        }
    }

    /// <summary>
    /// Verifica se o cursor do mouse/toque está sobre a área desenhável e, caso positivo,
    /// retorna a coordenada da textura para o desenho.
    /// </summary>
    /// <param name="pos">Parâmetro de saída que irá receber o <c>Vector3</c> com
    /// as coordenadas de textura para o desenho, caso o retorno seja verdadeiro. Se
    /// o retorno for falso, esse parâmetro recebe <c>Vector3.zero</c>.</param>
    /// <returns>Retorna verdadeiro se o cursor do mouse/toque está sobre a área
    /// desenhável, e falso caso contrário.</returns>
    private bool HitTestDrawableArea(out Vector3 pos)
    {
        pos = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        Vector3 cursorPos = new Vector3(mousePos.x, mousePos.y, 0.0f);

        RaycastHit hit;
        Ray cursorRay = Camera.main.ScreenPointToRay(cursorPos);
        if(Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider collider = hit.collider as MeshCollider;
            if(collider == null || collider.sharedMesh == null)
                return false;

            Vector2 texturePos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            pos.x = texturePos.x * 1.77f; // Multiplica por 1.77 por causa da razão de aspecto 16:9 da textura
            pos.y = texturePos.y;
            pos.z = 0.0f;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Constrói o sprite e a cor para a máscara de desenho de acordo com o brush, a cor e
    /// a ferramenta selecionados na UI pelo usuário.
    /// </summary>
    /// <param name="sprite">Parâmetro de saída que receberá o Sprite da máscara produzido.</param>
    /// <param name="color">Parâmetro de saída que receberá a cor para ser utilizada com o Sprite.</param>
    /// <returns>Verdadeiro se tudo correu bem, falso caso algum erro ocorreu (por exemplo, se
    /// um parâmetro da UI estiver errado por algum motivo). No caso de erros, a descrição
    /// é exibida no console.</returns>
    private bool buildMask(out Sprite sprite, out Color color)
    {
        sprite = null;
        color = Color.white;

        Tools tools = GetComponent<Tools>();
        Brushes brushes = GetComponent<Brushes>();

        // Obtém o sprite da máscara a ser usado de acordo com o brush selecionado
        int ind = brushes.validButtons().IndexOf(brushes.selected);
        if(ind >=0 && ind < masksPencil.Length && ind < masksPaintbrush.Length)
            sprite = (tools.selected == "paintbrush" ? masksPaintbrush[ind] : masksPencil[ind]);
        else
        {
            Debug.LogError("Brush inválido: " + brushes.selected);
            return false;
        }

        // Define a cor da máscara (se a ferramenta for a borracha, a cor é sempre branca)
        if(tools.selected != "eraser")
        {
            Colors colors = GetComponent<Colors>();
            try
            {
                color = (Color) typeof(Color).GetProperty(colors.selected).GetValue(null, null);
            }
            catch
            {
                sprite = null;
                Debug.LogError("Brush inválido: " + brushes.selected);
                return false;
            }
        }

        return true; 
    }

    /// <summary>
    /// Realiza o "bake" da textura. Isto é, cria uma nova textura de base (para o objeto
    /// na frente da câmera de renderização) com base no que está sendo "visto" por essa câmera,
    /// e então deleta as instâncias das máscaras utilizadas para simular o desenho.
    /// </summary>
    private void bakeMasksTexture()
    {
        // Cria uma nova textura o que está sendo exibido na área de renderização
        // (que contém a combinação de todas as máscaras de desenho posicionadas)
        RenderTexture.active = dynamicTexture;
        Texture2D tex = new Texture2D(dynamicTexture.width, dynamicTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, dynamicTexture.width, dynamicTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        // Define a textura criada como base para o objeto na frente da câmera de renderização
        baseMaterial.mainTexture = tex;

        // Destroy todos os objetos de máscaras existentes (já que a partir de então eles
        // são desnecessários)
        foreach (Transform child in brushContainer.transform)
            Destroy(child.gameObject);

        // Indica que o processo de baking terminou
        isBaking = false;
    }
}
