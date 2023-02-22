using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectionColor;
    [SerializeField] private ActionType[] actions;
    [SerializeField] private Color[] colors;

    private Dictionary<ActionType, Color> actionColors = new Dictionary<ActionType, Color>();
    private Cell highlightedCell;
    private Cell[] highlightedDangers = new Cell[0];

    private void Awake()
    {
        for (int i = 0; i < actions.Length; i++)
            actionColors.Add(actions[i], colors[i]);
    }

    public void HighlightDangers(Cell[] cells = null)
    {
        if (cells == highlightedDangers) return;

        foreach (Cell danger in highlightedDangers)
            danger.ResetColor();

        if (cells == null)
        {
            highlightedDangers = new Cell[0];
            return;
        }

        foreach (Cell cell in cells)
            cell.SetColor(actionColors[ActionType.Attack]);

        highlightedDangers = cells;
    }

    #region cell
    public void UpdateHighlight(Cell cell, Color color)
    {
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
        if (type == ActionType.None)
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
