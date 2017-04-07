using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Classe principal do controlador do jogo.
/// </summary>
public class GameController: MonoBehaviour
{
    /// <summary>
    /// Painel de exibição de informações sobre a aplicação.
    /// </summary>
    public GameObject infoPanel;

    /// <summary>
    /// Instância única do objeto de diálogo com o usuário.
    /// </summary>
    private Dialog dialog;

    /// <summary>
    /// Objeto de ação para o botão de confirmação do diálogo de encerramento da aplicação.
    /// </summary>
    private UnityAction confirmExit;

    /// <summary>
    /// Objeto de ação para o botão de negação do diálogo de encerramento da aplicação.
    /// </summary>
    private UnityAction ignoreExit;

	/// <summary>
    /// Método de inicialização do jogo (chamado uma vez no início).
    /// </summary>
	void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        dialog = Dialog.instance();
        confirmExit = new UnityAction(quit);
        ignoreExit = new UnityAction(ignore);
    }

    /// <summary>
    /// Método de execução do jogo (chamado a cada quadro).
    /// </summary>
    void Update()
    {
        // A tecla "escape" funciona como a tecla "voltar" nos dispositivos móveis
        if(Input.GetKey("escape"))
            terminate();
	}

    /// <summary>
    /// Exibe ou esconde a tela com as informações sobre a aplicação.
    /// </summary>
    public void toggleInfo()
    {
        infoPanel.SetActive(!infoPanel.activeSelf);
    }

    /// <summary>
    /// Realiza o encerramento do jogo (via tecla "voltar" ou via UI), solicitando ao
    /// usuário a confirmação por meio de uma tela de diálogo.
    /// </summary>
    public void terminate()
    {
        dialog.askYesNo("Você tem certeza que deseja fechar a aplicação?", confirmExit, ignoreExit);
    }

    /// <summary>
    /// Fecha a aplicação e retorna para o sistema operacional.
    /// </summary>
    protected void quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Método "dummy" utilizado pelo botão "Não" no diálogo de encerramento da aplicação
    /// (já que nessa ação a aplicação não precisa fazer nada).
    /// </summary>
    protected void ignore()
    {
    }
}
