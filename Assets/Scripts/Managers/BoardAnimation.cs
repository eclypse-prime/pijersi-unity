using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectionColor;
    [SerializeField] private Color singleActionColor;
    [SerializeField] private Color multipleActionColor;
    [SerializeField] private Color[] dangerColors;

    private Cell highlightedCell;
    private Cell[][] highlightedDangers = new Cell[3][];

    /// <summary>
    /// Shows dangerous pieces by changing the color of their cells.
    /// </summary>
    /// <param name="cells">Cells containing a piece that is dangerous for the selected top piece.</param>
    /// <param name="cells1">[optional] Cells containing a piece that is dangerous for the selected bottom piece (if it unstack).</param>
    /// <param name="cells2">[optional] Cells containing a piece that is dangerous for the selected bottom piece (if it stack move then unstack).</param>
    public void HighlightDangers(Cell[] cells, Cell[] cells1 = null, Cell[] cells2 = null)
    {
        if (cells == highlightedDangers[0] && cells1 == highlightedDangers[1] && cells2 == highlightedDangers[2]) return;

        // reset of all highlightedDangers
        foreach (Cell[] dangers in highlightedDangers)
        {
            if (dangers == null) continue;

            foreach (Cell danger in dangers)
                danger.ResetColor();
        }

        if (cells == null)
        {
            highlightedDangers = new Cell[3][];
            return;
        }

        foreach (Cell cell in cells)
            cell.SetColor(dangerColors[0]);
        highlightedDangers[0] = cells;

        highlightedDangers[1] = null;
        if (cells1 != null)
        {
            foreach (Cell cell in cells1)
                cell.SetColor(dangerColors[1]);
            highlightedDangers[1] = cells1;
        }

        if (cells2 == null)
        {
            highlightedDangers[2] = null;
            return;
        }

        foreach (Cell cell in cells2)
            cell.SetColor(dangerColors[2]);
        highlightedDangers[2] = cells2;
    }

    /// <summary>
    /// Changes the highlighted cell.
    /// </summary>
    public void UpdateHighlight(Cell cell, Color color)
    {
        highlightedCell?.ResetColor();

        highlightedCell = cell;
        highlightedCell.SetColor(color);
    }

    /// <inheritdoc cref="UpdateHighlight(Cell, Color)"/>
    public void UpdateHighlight(Cell cell)
    {
        UpdateHighlight(cell, highlightColor);
    }

    /// <inheritdoc cref="UpdateHighlight(Cell, Color)"/>
    public void UpdateHighlight(Cell cell, bool isSingleAction)
    {
        UpdateHighlight(cell, isSingleAction ? singleActionColor : multipleActionColor);
    }

    /// <summary>
    /// Sets the cell color to the selection color.
    /// </summary>
    public void NewSelection(Cell cell)
    {
        highlightedCell = null;
        cell.SetColor(selectionColor);
    }
}
