using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Singleton que implementa o acesso aos diálogos de interação com o usuário.
/// </summary>
public class Dialog : MonoBehaviour
{
    /// <summary>
    /// Objeto do diálogo na UI.
    /// </summary>
    public GameObject dialogObject;

    /// <summary>
    /// Objeto com o texto do diálogo.
    /// </summary>
    public Text textObject;

    /// <summary>
    /// Objeto do botão "Sim" do diálogo.
    /// </summary>
    public Button yesButton;

    /// <summary>
    /// Objeto com o botão "Não" do diálogo.
    /// </summary>
    public Button noButton;

    private static Dialog defaultInstance;

    /// <summary>
    /// Método estático de acesso à instância única do singleton.
    /// </summary>
    /// <returns>Retorna a instância única da classe <c>Dialog</c>.</returns>
    public static Dialog instance()
    {
        if(!defaultInstance)
        {
            defaultInstance = FindObjectOfType(typeof(Dialog)) as Dialog;
            if (!defaultInstance)
                Debug.LogError("Oops! Deve existir uma instância singleton da classe Dialog!");
        }
        return defaultInstance;
    }

    /// <summary>
    /// Apresenta o diálogo modal para o usuário com o texto dado e com os botões "Sim" e "Não".
    /// </summary>
    /// <param name="text">String com o texto a ser apresentado na janela de diálogo.</param>
    /// <param name="yesEvent">Instância de <c>UnityAction</c> com o evento a ser chamado no
    /// clique do botão "Sim".</param>
    /// <param name="noEvent">Instância de <c>UnityAction</c> com o evento a ser chamado no
    /// clique do botão "Não".</param>
	public void askYesNo(string text, UnityAction yesEvent, UnityAction noEvent)
    {
        yesButton.onClick.AddListener(yesEvent);
        yesButton.onClick.AddListener(close);
        
        noButton.onClick.AddListener(noEvent);
        noButton.onClick.AddListener(close);

        textObject.text = text;

        yesButton.enabled = true;
        noButton.enabled = true;
        dialogObject.SetActive(true);
    }

    /// <summary>
    /// Fecha o painel com o diálogo modal.
    /// </summary>
    void close()
    {
        textObject.text = "";

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.enabled = false;
        noButton.enabled = false;

        dialogObject.SetActive(false);
    }
}
