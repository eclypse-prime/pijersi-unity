using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Board : MonoBehaviour
{
    private const int columnCount = 7;
    private const int lineCount = 7;
    private const float columnStep = 2f;
    private const float lineStep = 1.73f;
    private const float PieceHeight = 1f;
    private readonly char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
    private readonly string[] darkCells = { "C3", "C4", "D3", "D5", "E3", "E4" };

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject[] piecePrefabs;
    [SerializeField] private Transform[] teams;
    [SerializeField] private int[] pieceCounts;
    [SerializeField] private Vector2Int[] starter;
    [Range(0f, 1f)]
    [SerializeField] private float PlacementRng;
    [SerializeField] private Material m_dark;
    [SerializeField] private Material m_black;
    [SerializeField] private Material m_white;

    private new Transform transform;
    private readonly List<Piece> deadPieces = new();

    public Cell[] Cells { get; private set; }
    public Piece[] Pieces { get; private set; }
    public int LineCount => lineCount;

    private void Awake()
    {
        transform = base.transform;
        
        BuildBoard();
        BuildTeams();
    }

    private void BuildBoard()
    {
        SpawnCells();
        SetNeighboursCells();
    }

    private void SpawnCells()
    {
        List<Cell> cells = new();
        int columnOffset = 0;
        float halfColumnStep = columnStep / 2;
        float columnStepOffset = 0f;
        for (int i = lineCount - 1; i >= 0; i--) // line
        {
            columnStepOffset = halfColumnStep - columnStepOffset;
            columnOffset = -1 - columnOffset;
            int lineSize = columnCount + columnOffset;
            for (int j = 0; j < lineSize; j++) // column
            {
                Vector3 position = new Vector3(columnStep * j + columnStepOffset, 0f, lineStep * i);
                Cell cell = Instantiate(cellPrefab, position, Quaternion.identity, transform).GetComponent<Cell>();
                cell.x = lineCount - 1 - i;
                cell.y = j;
                cell.name = letters[i] + (j + 1).ToString();

                if (i == 0 || i == lineCount - 1 || j == 0 || j == lineSize - 1 || IsDarkCell(cell.name))
                    cell.renderer.material = m_dark;

                cells.Add(cell);
            }
        }

        Cells = cells.ToArray();
    }

    private void SetNeighboursCells()
    {
        int columnOffset = 0;
        int cellId = 0;
        bool isAfterFirstLine = false;
        for (int i = 0; i < lineCount; i++)
        {
            int nextLineSize = columnCount + columnOffset;
            columnOffset = -1 - columnOffset;
            int lineSize = columnCount + columnOffset;
            bool isBeforeLastLine = i < lineCount - 1;
            bool isAfterLineStart = false;
            bool isLongLine = lineSize > nextLineSize;
            for (int j = 0; j < lineSize; j++)
            {
                bool isBeforeLineEnd = j < lineSize - 1;
                bool isNextLineEnd = j < nextLineSize;
                bool isBeforeNextLineEnd = j < nextLineSize - 1;
                Cell[] nears = new Cell[6];
                if (isAfterLineStart)
                    nears[2] = Cells[CoordsToIndex(i, j - 1)];
                if (isBeforeLineEnd)
                    nears[3] = Cells[CoordsToIndex(i, j + 1)];
                if (isLongLine)
                {
                    if (isAfterFirstLine)
                    {
                        if (isAfterLineStart)
                            nears[0] = Cells[CoordsToIndex(i - 1, j - 1)];
                        if (isNextLineEnd)
                            nears[1] = Cells[CoordsToIndex(i - 1, j)];
                    }
                    if (isBeforeLastLine)
                    {
                        if (isAfterLineStart)
                            nears[4] = Cells[CoordsToIndex(i + 1, j - 1)];
                        if (isNextLineEnd)
                            nears[5] = Cells[CoordsToIndex(i + 1, j)];
                    }

                    Cells[cellId++].SetNears(nears);
                    isAfterLineStart = true;
                    continue;
                }
                if (isAfterFirstLine)
                {
                    nears[0] = Cells[CoordsToIndex(i - 1, j)];
                    if (isBeforeNextLineEnd)
                        nears[1] = Cells[CoordsToIndex(i - 1, j + 1)];
                }
                if (isBeforeLastLine)
                {
                    nears[4] = Cells[CoordsToIndex(i + 1, j)];
                    if (isBeforeNextLineEnd)
                        nears[5] = Cells[CoordsToIndex(i + 1, j + 1)];
                }

                Cells[cellId++].SetNears(nears);
                isAfterLineStart = true;
            }
            isAfterFirstLine = true;
        }
    }

    private void BuildTeams()
    {
        Pieces = new Piece[starter.Length];
        int pieceId = 0;
        for (int i = 0; i < teams.Length; i++) // teams
        {
            for (int j = 0; j < piecePrefabs.Length; j++) // types
            {
                for (int k = 0; k < pieceCounts[j]; k++) // copies
                    SpawnPiece(i, j);
            }
        }

        void SpawnPiece(int i, int j)
        {
            Vector2Int pos = starter[pieceId];
            Piece piece = Instantiate(piecePrefabs[j], Vector3.zero, Quaternion.identity, teams[i]).GetComponent<Piece>();
            piece.team = i;

            Pieces[pieceId] = piece;
            Cell cell = Cells[CoordsToIndex(pos.x, pos.y)];

            MovePieceToCell(piece, cell);

            if (i > 0)
            {
                piece.mainRenderer.material = m_black;
                foreach (MeshRenderer sign in piece.SignRenderer)
                    sign.material = m_white;
            }

            pieceId++;
        }
    }

    private void MovePieceToCell(Piece piece, Cell cell)
    {
        cell.pieces[cell.IsEmpty ? 0 : 1] = piece;
        piece.MoveTo(cell, PlacementRng, cell.IsFull ? PieceHeight : 0);
    }

    private bool IsDarkCell(string name)
    {
        foreach (string darkCell in darkCells)
        {
            if (name == darkCell)
                return true;
        }

        return false;
    }
}