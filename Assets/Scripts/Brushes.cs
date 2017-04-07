using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de seleção de brushes.
/// </summary>
public class Brushes: AbstractMenu
{
    /// <summary>
    /// Lista com os nomes dos brushes válidos.
    /// </summary>
    private List<string> buttons = new List<string>(new string[] {
            "plus", "vertical", "horizontal", "circle", "gradient"
    });

    /// <summary>
    /// Devolve a lista de brushes válidos.
    /// </summary>
    /// <returns>Lista de strings com os nomes dos brushes válidos neste menu.</returns>
    protected override List<string> validButtons()
    {
        return buttons;
    }
}