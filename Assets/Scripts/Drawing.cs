using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implementa o desenho sobre a folha de papel.
/// </summary>
public class Drawing: MonoBehaviour
{
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

    private float lastZ;

    /// <summary>
    /// Inicialização da classe.
    /// </summary>
    void Start()
    {
        basePos = brushContainer.transform.position;
        isDrawing = false;
        cursor.SetActive(false);
        lastZ = 0.001f;

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
        if(HitTestDrawableArea(out pos))
        {
            // Exibe o cursor (apenas em plataformas não móveis)
            if(!isMobile)
            {
                cursor.SetActive(true);
                cursor.transform.position = pos + basePos;
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

        pos.z = lastZ - 0.001f;

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

        // Se a ferramenta selecionada é o pincel, aplica um filtro Gaussiano à máscara        
        if (tools.selected == "paintbrush")
        {
            // TODO
        }

        return true; 
    }
}
