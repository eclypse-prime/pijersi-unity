using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int c_cellCountX = 7;
    private const int c_cellCountZ = 7;
    private const float c_stepX = 2f;
    private const float c_stepZ = 1.73f;
    private readonly char[] r_letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
    private readonly string[] r_darkCells = { "C2", "C3", "D2", "D4", "E2", "E3" };

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject[] piecePrefabs;
    [SerializeField] private Transform[] teams;
    [SerializeField] private int[] pieceCounts;
    [SerializeField] private Vector2Int[] starter;
    [SerializeField] private Material m_dark;
    [SerializeField] private Material m_black;
    [SerializeField] private Color highlightColor;

    private new Transform transform;
    private Cell[][] cells;

    private void Awake()
    {
        transform = base.transform;
    }

    private void Start()
    {
        BuildBoard();
        BuildTeams();
    }

    #region build
    private void BuildBoard()
    {
        float halfStepX     = c_stepX / 2;
        float stepOffsetX   = 0f;
        int countOffestX    = 0;
        cells               = new Cell[c_cellCountZ][];
        for (int i = 0; i < c_cellCountZ; i++) // axe Z
        {
            stepOffsetX     = halfStepX - stepOffsetX;
            countOffestX    = -1 - countOffestX;
            int lineSize    = c_cellCountX + countOffestX;
            cells[i]        = new Cell[lineSize];
            for (int j = 0; j < lineSize; j++) // axe X
            {
                Vector3 position = new Vector3(c_stepX * j + stepOffsetX, 0f, c_stepZ * i);
                Cell cell = Instantiate(cellPrefab, position, Quaternion.identity, transform).GetComponent<Cell>();
                cell.gameObject.name = r_letters[i] + j.ToString();

                if (i == 0 || i == c_cellCountZ - 1 || j == 0 || j == lineSize - 1 || IsDarkCell(cell.name))
                    cell.renderer.material = m_dark;

                cells[i][j] = cell.GetComponent<Cell>();
            }
        }
    }

    private void BuildTeams()
    {
        int pieceId = 0;
        for (int i = 0; i < teams.Length; i++) // équipes
        {
            for (int j = 0; j < piecePrefabs.Length; j++) // types
            {
                for (int k = 0; k < pieceCounts[j]; k++) // copies
                {
                    Vector2Int coordinate   = starter[pieceId++];
                    Vector3 position        = cells[coordinate.x][coordinate.y].transform.position;

                    GameObject piece        = Instantiate(piecePrefabs[j], position, Quaternion.identity, teams[i]);

                    if (i > 0)
                        piece.GetComponentInChildren<MeshRenderer>().material = m_black;
                }
            }
        }
    }

    private bool IsDarkCell(string name)
    {
        foreach (string darkCell in r_darkCells)
        {
            if (name == darkCell)
                return true;
        }

        return false;
    }
    #endregion
}
