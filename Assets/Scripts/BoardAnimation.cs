using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color selectionColor;

    private Cell highlightedCell;

    #region cell
    public void UpdateHighlight(Transform transform)
    {
        if (highlightedCell != null && transform == highlightedCell.transform) return;

        if (highlightedCell != null)
            highlightedCell.ResetColor();

        highlightedCell = transform.GetComponent<Cell>();
        highlightedCell.SetColor(highlightColor);
    }

    public void NewSelection(Cell cell)
    {
        highlightedCell = null;
        cell.SetColor(selectionColor);
    }
    #endregion
}
