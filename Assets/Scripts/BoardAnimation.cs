using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnimation : MonoBehaviour
{
    [SerializeField] private Color highlightColor;

    private Cell highlightedCell;

    #region cell
    public void UpdateHighlight(Transform transform)
    {
        if (highlightedCell != null && transform == highlightedCell.transform) return;

        if (highlightedCell != null)
            highlightedCell.ResetColor();

        if (transform != null)
        {
            highlightedCell = transform.GetComponent<Cell>();
            highlightedCell.SetColor(highlightColor);
        }
    }
    #endregion
}
