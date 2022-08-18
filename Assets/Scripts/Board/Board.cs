using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int columnCount       = 7;
    private const int lineCount         = 7;
    private const float columnStep      = 2f;
    private const float lineStep        = 1.73f;
    private const float PieceHeight     = 1f;
    private readonly char[] letters     = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
    private readonly string[] darkCells = { "C2", "C3", "D2", "D4", "E2", "E3" };

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
    private Cell[] cells;
    private Piece[] pieces;

    public int LineCount => lineCount;

    #region base
    private void Awake()
    {
        transform = base.transform;
        BuildBoard();
        BuildTeams();
    }
    #endregion

    #region build
    private void BuildBoard()
    {
        float halfColumnStep    = columnStep / 2;
        float columnStepOffset  = 0f;
        int columnOffset        = 0;
        List<Cell> cells = new List<Cell>();
        for (int i = lineCount - 1; i >= 0; i--) // line
        {
            columnStepOffset = halfColumnStep - columnStepOffset;
            columnOffset     = -1 - columnOffset;
            int lineSize     = columnCount + columnOffset;
            for (int j = 0; j < lineSize; j++) // column
            {
                Vector3 position = new Vector3(columnStep * j + columnStepOffset, 0f, lineStep * i);
                Cell cell        = Instantiate(cellPrefab, position, Quaternion.identity, transform).GetComponent<Cell>();
                cell.x           = i;
                cell.y           = j;
                cell.name        = letters[i] + j.ToString();

                if (i == 0 || i == lineCount - 1 || j == 0 || j == lineSize - 1 || IsDarkCell(cell.name))
                    cell.renderer.material = m_dark;

                cells.Add(cell);
            }
        }

        // set cells neighbours
        columnOffset = 0;
        int cellId = 0;
        for (int i = 0; i < lineCount; i++)
        {
            int nextLineSize = columnCount + columnOffset;
            columnOffset     = -1 - columnOffset;
            int lineSize     = columnCount + columnOffset;
            for (int j = 0; j < lineSize; j++)
            {
                Cell[] nears = new Cell[6];
                if (j > 0)
                    nears[2] = cells[CoordsToIndex(i, j - 1)];
                if (j < lineSize - 1)
                    nears[3] = cells[CoordsToIndex(i, j + 1)];
                if (lineSize > nextLineSize)
                {
                    if (i > 0)
                    {
                        if (j > 0)
                            nears[0] = cells[CoordsToIndex(i - 1, j - 1)];
                        if (j < nextLineSize)
                            nears[1] = cells[CoordsToIndex(i - 1, j)];
                    }
                    if (i < lineCount - 1)
                    {
                        if (j > 0)
                            nears[4] = cells[CoordsToIndex(i + 1, j - 1)];
                        if (j < nextLineSize)
                            nears[5] = cells[CoordsToIndex(i + 1, j)];
                    }
                }
                else
                {
                    if (i > 0)
                    {
                        nears[0] = cells[CoordsToIndex(i - 1, j)];
                        if (j < nextLineSize - 1)
                            nears[1] = cells[CoordsToIndex(i - 1, j + 1)];
                    }
                    if (i < lineCount - 1)
                    {
                        nears[4] = cells[CoordsToIndex(i + 1, j)];
                        if (j < nextLineSize - 1)
                            nears[5] = cells[CoordsToIndex(i + 1, j + 1)];
                    }
                }

                cells[cellId++].SetNears(nears);
            }
        }

        this.cells = cells.ToArray();
    }

    private void BuildTeams()
    {
        pieces = new Piece[starter.Length];
        int pieceId = 0;
        for (int i = 0; i < teams.Length; i++) // équipes
        {
            for (int j = 0; j < piecePrefabs.Length; j++) // types
            {
                for (int k = 0; k < pieceCounts[j]; k++) // copies
                {
                    Vector2Int pos  = starter[pieceId];
                    Piece piece     = Instantiate(piecePrefabs[j], Vector3.zero, Quaternion.identity, teams[i]).GetComponent<Piece>();
                    piece.team      = i;

                    pieces[pieceId] = piece;
                    Cell cell = cells[CoordsToIndex(pos.x, pos.y)];

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
        }
    }

    public void ResetBoard()
    {
        foreach (Piece piece in pieces)
        {
            piece.cell.pieces = new Piece[2];
            piece.gameObject.SetActive(true);
        }

        for (int i = 0; i < pieces.Length; i++)
            MovePieceToCell(pieces[i], cells[CoordsToIndex(starter[i].x, starter[i].y)]);
    }

    private void MovePieceToCell(Piece piece, Cell cell)
    {
        cell.pieces[cell.isEmpty ? 0 : 1] = piece;
        piece.MoveTo(cell, PlacementRng, cell.isFull ? PieceHeight : 0);
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

    private int CoordsToIndex(int x, int y)
    {
        if (x % 2 == 0)
            return (columnCount * 2 - 1) * x / 2 + y;

        return (columnCount * 2 - 1) * (x - 1) / 2 + columnCount - 1 + y;
    }
    #endregion

    #region Action
    public void Move(Cell start, Cell end)
    {
        end.pieces   = start.pieces;
        start.pieces = new Piece[2];

        end.pieces[0].InitMove(end, PlacementRng);
        end.pieces[1]?.InitMove(end, PlacementRng);
    }

    public void Attack(Cell start, Cell end)
    {
        end.pieces[0].gameObject.SetActive(false);
        end.pieces[1]?.gameObject.SetActive(false);
        Move(start, end);
    }

    public void Stack(Cell start, Cell end)
    {
        int startId   = start.isFull ? 1 : 0;
        end.pieces[1] = start.pieces[startId];
        start.pieces[startId] = null;

        end.pieces[1].InitMove(end, PlacementRng, PieceHeight);
    }

    public void Unstack(Cell start, Cell end)
    {
        if (!end.isEmpty)
        {
            end.pieces[0].gameObject.SetActive(false);
            end.pieces[1]?.gameObject.SetActive(false);
        }

        end.pieces[0]   = start.pieces[1];
        end.pieces[1]   = null;
        start.pieces[1] = null;

        end.pieces[0].InitMove(end, PlacementRng, 0f);
    }

    public bool UpdateMove(Cell cell)
    {
        bool firstUpdate = cell.pieces[0].UptadeMove();
        bool secondUpdate = cell.pieces[1]?.UptadeMove() == true;
        return firstUpdate || secondUpdate;
    }
    #endregion
}
