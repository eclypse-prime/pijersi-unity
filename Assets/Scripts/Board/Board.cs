using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int cellCountX        = 7;
    private const int cellCountY        = 7;
    private const float stepX           = 2f;
    private const float stepY           = 1.73f;
    private const float PieceHeight     = 1f;
    private readonly char[] letters     = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
    private readonly string[] darkCells = { "C2", "C3", "D2", "D4", "E2", "E3" };

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject[] piecePrefabs;
    [SerializeField] private Transform[] teams;
    [SerializeField] private int[] pieceCounts;
    [SerializeField] private Vector2Int[] starter;
    [SerializeField] private Material m_dark;
    [SerializeField] private Material m_black;
    [SerializeField] private Material m_white;

    private new Transform transform;
    private Cell[][] cells;
    private Piece[] pieces;

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
        float halfStepX     = stepX / 2;
        float stepXOffset   = 0f;
        int countXOffest    = 0;
        cells               = new Cell[cellCountY][];
        for (int i = 0; i < cellCountY; i++) // axe Z
        {
            stepXOffset     = halfStepX - stepXOffset;
            countXOffest    = -1 - countXOffest;
            int lineSize    = cellCountX + countXOffest;
            int x           = cellCountY - 1 - i;
            cells[x]        = new Cell[lineSize];
            for (int j = 0; j < lineSize; j++) // axe X
            {
                Vector3 position = new Vector3(stepX * j + stepXOffset, 0f, stepY * i);
                Cell cell        = Instantiate(cellPrefab, position, Quaternion.identity, transform).GetComponent<Cell>();
                cell.x           = x;
                cell.y           = j;
                cell.name        = letters[i] + j.ToString();

                if (i == 0 || i == cellCountY - 1 || j == 0 || j == lineSize - 1 || IsDarkCell(cell.name))
                    cell.renderer.material = m_dark;

                cells[cell.x][cell.y] = cell;
            }
        }
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
                    Cell cell = cells[pos.x][pos.y];
                    if (cell.isEmpty)
                    {
                        piece.MoveTo(cell);
                        cell.pieces[0] = piece;
                    }
                    else
                    {
                        piece.MoveTo(cell, PieceHeight);
                        cell.pieces[1] = piece;
                    }

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

    private bool IsDarkCell(string name)
    {
        foreach (string darkCell in darkCells)
        {
            if (name == darkCell)
                return true;
        }

        return false;
    }
    #endregion

    #region Action
    public void Move(Cell start, Cell end)
    {
        end.pieces = start.pieces;
        start.pieces = new Piece[2];

        end.pieces[0].MoveTo(end);
        end.pieces[1]?.MoveTo(end, PieceHeight);
    }

    public void Attack(Cell start, Cell end)
    {
        end.pieces[0].gameObject.SetActive(false);
        end.pieces[1]?.gameObject.SetActive(false);

        Move(start, end);
    }

    public void StackUnstask(Cell start, Cell end)
    {
        int startId = start.isFull ? 1 : 0;
        int endId = end.isEmpty ? 0 : 1;
        end.pieces[endId] = start.pieces[startId];
        start.pieces[startId] = null;

        end.pieces[endId].MoveTo(end, endId);
    }
    #endregion

    #region get
    public Dictionary<ActionType, List<Cell>> GetValideMoves(Cell cell, bool canMove, bool canStack)
    {
        Cell[] neighbours = GetNeighbours(cell);
        return cell.lastPiece.GetValideMoves(neighbours, canMove, canStack);
    }

    public Cell[] GetNeighbours(Cell cell)
    {
        List<Cell> result = new List<Cell>();
        foreach (Vector2Int pos in cell.GetPossibleNeighbours())
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= cells.Length || pos.y >= cells[pos.x].Length)
                continue;

            result.Add(cells[pos.x][pos.y]);
        }

        return result.ToArray();
    }
    #endregion

    public bool IsWin(Cell cell)
    {
        if (cell.x == cellCountX && cell.pieces[0].team == 0 || cell.x == 0 && cell.pieces[0].team == 1)
            return true;

        return false;
    }
}
