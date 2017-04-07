using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de seleção de ferramentas.
/// </summary>
public class Tools: AbstractMenu
{
    /// <summary>
    /// Lista com os nomes das ferramentas válidas.
    /// </summary>
    private List<string> buttons = new List<string>(new string[] {
            "pencil", "paintbrush", "eraser"
    });

    /// <summary>
    /// Devolve a lista de ferramentas válidas.
    /// </summary>
    /// <returns>Lista de strings com os nomes das ferramentas válidas neste menu.</returns>
    protected override List<string> validButtons()
    {
        return buttons;
    }
}