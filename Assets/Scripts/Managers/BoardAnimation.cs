using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectionColor;
    [SerializeField] private Color singleActionColor;
    [SerializeField] private Color multipleActionColor;
    [SerializeField] private Color[] dangerColors;

    private Cell highlightedCell;
    private readonly Cell[][] highlightedDangers = new Cell[3][];

    /// <summary>
    /// Shows dangerous pieces by changing the color of their cells.
    /// </summary>
    /// <param name="cells">Cells containing a piece that is dangerous for the selected top piece.</param>
    /// <param name="cells1">[optional] Cells containing a piece that is dangerous for the selected bottom piece (if it unstack).</param>
    /// <param name="cells2">[optional] Cells containing a piece that is dangerous for the selected bottom piece (if it stack move then unstack).</param>
    public void HighlightDangers(Cell[] cells, Cell[] cells1 = null, Cell[] cells2 = null)
    {
        if (cells == highlightedDangers[0] && cells1 == highlightedDangers[1] && cells2 == highlightedDangers[2]) return;

        ResetHighlights();

        Highlight(cells, 0);
        Highlight(cells1, 1);
        Highlight(cells2, 2);

        void ResetHighlights()
        {
            foreach (Cell[] dangers in highlightedDangers)
            {
                if (dangers == null) continue;

                foreach (Cell danger in dangers)
                    danger.ResetColor();
            }
        }

        void Highlight(Cell[] dangers, int id)
        {
            if (dangers == null)
            {
                highlightedDangers[id] = null;
                return;
            }

            foreach (Cell danger in dangers)
                danger.SetColor(dangerColors[id]);
            highlightedDangers[id] = dangers;
        }
    }

    public void UpdateHighlight(Cell cell, Color color)
    {
        highlightedCell?.ResetColor();
        highlightedCell = cell;
        highlightedCell.SetColor(color);
    }

    public void UpdateHighlight(Cell cell) => 
        UpdateHighlight(cell, highlightColor);
    public void UpdateHighlight(Cell cell, bool isSingleAction) => 
        UpdateHighlight(cell, isSingleAction ? singleActionColor : multipleActionColor);

    /// <summary>
    /// Sets the cell color to the selection color.
    /// </summary>
    public void NewSelection(Cell cell)
    {
        highlightedCell = null;
        cell.SetColor(selectionColor);
    }
}
