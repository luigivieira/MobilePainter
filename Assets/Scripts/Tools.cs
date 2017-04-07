using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla a seleção de ferramentas.
/// </summary>
public class Tools: MonoBehaviour
{
    /// <summary>
    /// Painel de exibição dos botões de ferramenta.
    /// </summary>
    public GameObject toolPanel;

    /// <summary>
    /// Botão de seleção de ferramentas na interface principal da aplicação.
    /// </summary>
    public Button toolButton;

    /// <summary>
    /// Define a ferramenta atualmente em uso para desenho.
    /// </summary>
    private string currentTool;

    /// <summary>
    /// Lista com as ferramentas aceitáveis na interface da aplicação.
    /// </summary>
    private List<string> validTools;

    /// <summary>
    /// Lista de sprites das ferramentas do botão de seleção na interface principal da aplicação.
    /// </summary>
    public Sprite[] toolSprites;

    /// <summary>
    /// Método de inicialização.
    /// </summary>
    void Start()
    {
        validTools = new List<string>(new string[] {
            "pencil", "paintbrush", "eraser"
        });

        toolButton.GetComponent<Image>().sprite = toolSprites[0];
    }

    /// <summary>
    /// Define a nova ferramenta para desenho.
    /// </summary>
    /// <param name="toolName">Nome da nova ferramenta a ser utilizada para desenho.
    /// Os valores possíveis são: "pencil", "paintbrush" e "eraser".
    /// </param>
    public void changeTool(string toolName)
    {
        closePanel();

        toolName = toolName.ToLower();
        if (validTools.Contains(toolName))
        {
            currentTool = toolName;
            int index = validTools.IndexOf(currentTool);
            toolButton.GetComponent<Image>().sprite = toolSprites[index];
        }
        else
            Debug.LogError("Valor inválido para ferramenta: " + toolName);
    }

    /// <summary>
    /// Abre o painel de seleção de ferramentas.
    /// </summary>
    public void openPanel()
    {
        toolPanel.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel de seleção de ferramentas.
    /// </summary>
    public void closePanel()
    {
        toolPanel.SetActive(false);
    }
}