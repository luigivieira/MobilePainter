using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Controla a seleção de cores.
/// </summary>
public class Colors: MonoBehaviour
{
    /// <summary>
    /// Painel de exibição dos botões de cor.
    /// </summary>
    public GameObject colorPanel;

    /// <summary>
    /// Botão de seleção de cores na interface principal da aplicação.
    /// </summary>
    public Button colorButton;

    /// <summary>
    /// Define a cor atualmente em uso para desenho.
    /// </summary>
    private string currentColor;

    /// <summary>
    /// Lista com as cores aceitáveis na interface da aplicação.
    /// </summary>
    private List<string> validColors;

    /// <summary>
    /// Lista de sprites das cores do botão de seleção na interface principal da aplicação.
    /// </summary>
    public Sprite[] colorSprites;

	/// <summary>
    /// Método de inicialização.
    /// </summary>
	void Start()
    {
        validColors = new List<string>(new string[] {
            "red", "green", "blue", "yellow", "cyan", "magenta"
        });

        colorButton.GetComponent<Image>().sprite = colorSprites[0];
    }
	
    /// <summary>
    /// Define a nova cor para desenho.
    /// </summary>
    /// <param name="colorName">Nome da nova cor a ser utilizada para desenho.
    /// Os valores possíveis são: "red", "green", "blue", "yellow", "cyan" e "magenta".
    /// </param>
    public void changeColor(string colorName)
    {
        closePanel();

        colorName = colorName.ToLower();
        if(validColors.Contains(colorName))
        {
            currentColor = colorName;
            int index = validColors.IndexOf(currentColor);            
            colorButton.GetComponent<Image>().sprite = colorSprites[index];
        }
        else
            Debug.LogError("Valor inválido para cor: " + colorName);
    }

    /// <summary>
    /// Abre o painel de seleção de cores.
    /// </summary>
    public void openPanel()
    {
        colorPanel.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel de seleção de cores.
    /// </summary>
    public void closePanel()
    {
        colorPanel.SetActive(false);
    }
}
