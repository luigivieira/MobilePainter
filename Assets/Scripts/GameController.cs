using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe principal do controlador do jogo.
/// </summary>
public class GameController: MonoBehaviour
{
	/// <summary>
    /// Método de inicialização do jogo (chamado uma vez no início).
    /// </summary>
	void Start()
    {
		
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
    /// Realiza o encerramento do jogo (via tecla "voltar" ou via UI).
    /// Por segurança, o usuário é solicitado a confirmar o encerramento.
    /// </summary>
    public void terminate()
    {
        // TODO: Questionar encerramento
        Application.Quit();
    }
}
