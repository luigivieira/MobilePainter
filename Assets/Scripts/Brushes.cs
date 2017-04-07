using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla a seleção de brushes.
/// </summary>
public class Brushes: MonoBehaviour
{
    /// <summary>
    /// Painel de exibição dos botões de brush.
    /// </summary>
    public GameObject brushPanel;

    /// <summary>
    /// Botão de seleção de brushes na interface principal da aplicação.
    /// </summary>
    public Button brushButton;

    /// <summary>
    /// Define o brush atualmente em uso para desenho.
    /// </summary>
    private string currentBrush;

    /// <summary>
    /// Lista com os brushes aceitáveis na interface da aplicação.
    /// </summary>
    private List<string> validBrushes;

    /// <summary>
    /// Lista de sprites dos brushes do botão de seleção na interface principal da aplicação.
    /// </summary>
    public Sprite[] brushSprites;

    /// <summary>
    /// Método de inicialização.
    /// </summary>
    void Start()
    {
        validBrushes = new List<string>(new string[] {
            "plus", "vertical", "horizontal", "circle", "gradient"
        });

        brushButton.GetComponent<Image>().sprite = brushSprites[0];
    }

    /// <summary>
    /// Define o novo brush para desenho.
    /// </summary>
    /// <param name="brushName">Nome do novo brush a ser utilizado para desenho.
    /// Os valores possíveis são: "plus", "vertical", "horizontal", "circle" e "gradient".
    /// </param>
    public void changeBrush(string brushName)
    {
        closePanel();

        brushName = brushName.ToLower();
        if (validBrushes.Contains(brushName))
        {
            currentBrush = brushName;
            int index = validBrushes.IndexOf(currentBrush);
            brushButton.GetComponent<Image>().sprite = brushSprites[index];
        }
        else
            Debug.LogError("Valor inválido para brush: " + brushName);
    }

    /// <summary>
    /// Abre o painel de seleção de brushes.
    /// </summary>
    public void openPanel()
    {
        brushPanel.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel de seleção de brushes.
    /// </summary>
    public void closePanel()
    {
        brushPanel.SetActive(false);
    }
}