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
    /// Inicialização da classe.
    /// </summary>
    void Start ()
    {
        basePos = brushContainer.transform.position;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        isMobile = true;
#else
        isMobile = false;
#endif
    }
	
	/// <summary>
    /// Atualização a cada quadro do jogo.
    /// </summary>
	void Update ()
    {
        Vector3 worldPos;
        if(HitTestTexturePos(out worldPos))
        {
            // Exibe o cursor, se não estiver numa plataforma móvel
            if(!isMobile)
            {
                cursor.SetActive(true);
                cursor.transform.position = worldPos + basePos;
            }
        }
        else
            cursor.SetActive(false); // Esconde o cursor
    }

    /// <summary>
    /// Verifica se o cursor do mouse/toque está sobre a área desenhável e, caso positivo,
    /// retorna a coordenada da textura para o desenho.
    /// </summary>
    /// <param name="worldPos">Parâmetro de saída que irá receber o <c>Vector3</c> com
    /// as coordenadas de textura para o desenho, caso o retorno seja verdadeiro. Se
    /// o retorno for falso, esse parâmetro recebe <c>Vector3.zero</c>.</param>
    /// <returns>Retorna verdadeiro se o cursor do mouse/toque está sobre a área
    /// desenhável, e falso caso contrário.</returns>
    private bool HitTestTexturePos(out Vector3 worldPos)
    {
        worldPos = Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        Vector3 cursorPos = new Vector3(mousePos.x, mousePos.y, 0.0f);

        RaycastHit hit;
        Ray cursorRay = Camera.main.ScreenPointToRay(cursorPos);
        if(Physics.Raycast(cursorRay, out hit, 200))
        {
            MeshCollider collider = hit.collider as MeshCollider;
            if(collider == null || collider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            worldPos.x = pixelUV.x * 1.77f; // Multiplica por 1.77 por causa da razão de aspecto 16:9 da textura
            worldPos.y = pixelUV.y;
            worldPos.z = 0.0f;
            return true;
        }
        else
            return false;
    }
}
