using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla a seleção de ferramentas em um menu na UI.
/// Esta é uma classe abstrata que deve ser herdada por cada implementação específica
/// de menus.
/// </summary>
abstract public class AbstractMenu: MonoBehaviour
{
    /// <summary>
    /// Painel (menu) de exibição dos botões de seleção.
    /// </summary>
    public GameObject menuPanel;

    /// <summary>
    /// Botão de ativação do menu na interface principal.
    /// </summary>
    public Button activateButton;

    /// <summary>
    /// Lista de sprites dos botões de seleção na interface principal.
    /// </summary>
    public Sprite[] buttonSprites;

    /// <summary>
    /// Contém o botão selecionado no menu.
    /// </summary>
    protected string selectedButton;

    /// <summary>
    /// Método abstrato que devolve a lista de botões válidos (isto é, esperados por este menu).
    /// Este método deve ser implementado pelas classes filhas.
    /// </summary>
    /// <returns>Lista de strings com os nomes dos botões válidos neste menu.
    /// Obsevação: os nomes devem ser utilizados sempre em minúsculas.</returns>
    protected abstract List<string> validButtons();

    /// <summary>
    /// Inicialização do menu.
    /// </summary>
    protected void Awake()
    {
        activateButton.GetComponent<Image>().sprite = buttonSprites[0];
    }

    /// <summary>
    /// Altera o botão selecionado no menu.
    /// </summary>
    /// <param name="buttonName">Nome do novo botão selecionado no menu 
    /// (de acordo com as opções válidas devolvidas pelo método <c>validButtons</c>).
    /// </param>
    public void changeSelection(string buttonName)
    {
        closeMenu();

        buttonName = buttonName.ToLower();
        if (validButtons().Contains(buttonName))
        {
            selectedButton = buttonName;
            int index = validButtons().IndexOf(selectedButton);
            activateButton.GetComponent<Image>().sprite = buttonSprites[index];
        }
        else
            Debug.LogError("Botão inválido: " + buttonName);
    }

    /// <summary>
    /// Abre o painel do menu.
    /// </summary>
    public void openMenu()
    {
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel do menu.
    /// </summary>
    public void closeMenu()
    {
        menuPanel.SetActive(false);
    }
}