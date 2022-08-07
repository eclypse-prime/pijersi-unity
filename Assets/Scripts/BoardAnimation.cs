using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BoardAnimation : SerializedMonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectionColor;
    [SerializeField] private Dictionary<ActionType, Color> actionColors = new Dictionary<ActionType, Color>();

    private Cell highlightedCell;

    #region cell
    public void UpdateHighlight(Cell cell, Color color)
    {
        if (cell == highlightedCell) return;

        highlightedCell?.ResetColor();

        highlightedCell = cell;
        highlightedCell.SetColor(color);
    }

    public void UpdateHighlight(Cell cell)
    {
        UpdateHighlight(cell, highlightColor);
    }

    public void UpdateHighlight(Cell cell, ActionType type)
    {
        if (type == ActionType.none)
        {
            UpdateHighlight(cell);
            return;
        }

        UpdateHighlight(cell, actionColors[type]);
    }

    public void NewSelection(Cell cell)
    {
        highlightedCell = null;
        cell.SetColor(selectionColor);
    }
    #endregion
}
