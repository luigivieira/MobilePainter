using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de seleção de cores.
/// </summary>
public class Colors: AbstractMenu
{
    /// <summary>
    /// Lista com os nomes das cores válidas.
    /// </summary>
    private List<string> buttons = new List<string>(new string[] {
            "red", "green", "blue", "yellow", "cyan", "magenta"
    });

    /// <summary>
    /// Devolve a lista de cores válidas.
    /// </summary>
    /// <returns>Lista de strings com os nomes das cores válidas neste menu.</returns>
    protected override List<string> validButtons()
    {
        return buttons;
    }
}
