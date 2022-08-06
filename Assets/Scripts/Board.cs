using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int cellCountX        = 7;
    private const int cellCountZ        = 7;
    private const float stepX           = 2f;
    private const float stepZ           = 1.73f;
    private const float PieceHeight     = .5f;
    private readonly char[] letters     = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
    private readonly string[] darkCells = { "C2", "C3", "D2", "D4", "E2", "E3" };

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject[] piecePrefabs;
    [SerializeField] private Transform[] teams;
    [SerializeField] private int[] pieceCounts;
    [SerializeField] private Vector2Int[] starter;
    [SerializeField] private Material m_dark;
    [SerializeField] private Material m_black;

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
        float halfStepX     = stepX / 2;
        float stepXOffset   = 0f;
        int countXOffest    = 0;
        cells               = new Cell[cellCountZ][];
        for (int i = 0; i < cellCountZ; i++) // axe Z
        {
            stepXOffset     = halfStepX - stepXOffset;
            countXOffest    = -1 - countXOffest;
            int lineSize    = cellCountX + countXOffest;
            int x = cellCountZ - 1 - i;
            cells[x]        = new Cell[lineSize];
            for (int j = 0; j < lineSize; j++) // axe X
            {
                Vector3 position = new Vector3(stepX * j + stepXOffset, 0f, stepZ * i);
                Cell cell   = Instantiate(cellPrefab, position, Quaternion.identity, transform).GetComponent<Cell>();
                cell.x      = x;
                cell.y      = j;
                cell.gameObject.name = letters[i] + j.ToString();

                if (i == 0 || i == cellCountZ - 1 || j == 0 || j == lineSize - 1 || IsDarkCell(cell.name))
                    cell.GetComponentInChildren<Renderer>().material = m_dark;

                cells[cell.x][cell.y] = cell;
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
                    Vector2Int pos      = starter[pieceId++];
                    Vector3 position    = cells[pos.x][pos.y].transform.position;
                    Piece piece         = Instantiate(piecePrefabs[j], position, Quaternion.identity, teams[i]).GetComponent<Piece>();
                    piece.team          = i;

                    if (cells[pos.x][pos.y].isEmpty)
                        cells[pos.x][pos.y].pieces[0] = piece;
                    else
                        cells[pos.x][pos.y].pieces[1] = piece;

                    if (i > 0)
                        piece.GetComponentInChildren<MeshRenderer>().material = m_black;
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

    #region Piece
    public void MovePiece(Cell start, Cell destination)
    {
        destination.pieces = start.pieces;
        start.pieces = new Piece[2];

        Vector3 position = destination.transform.position;
        destination.pieces[0].transform.position = new Vector3(position.x, destination.pieces[0].transform.position.y, position.z);

        if (destination.isFull)
            destination.pieces[1].transform.position = new Vector3(position.x, destination.pieces[1].transform.position.y, position.z);
    }
    #endregion

    #region get
    public List<Cell> GetValideMoves(Cell cell)
    {
        List<Cell> result = GetNeighbours(cell);
        foreach (Cell neighbour in result.ToArray())
        {
            if (!neighbour.isEmpty)
                result.Remove(neighbour);
        }

        return result;
    }

    public List<Cell> GetNeighbours(Cell cell)
    {
        List<Cell> result = new List<Cell>();
        foreach (Vector2Int pos in GetPossibleNeighbours(cell))
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= cells.Length || pos.y >= cells[pos.x].Length)
                continue;

            result.Add(cells[pos.x][pos.y]);
        }

        return result;
    }

    public Vector2Int[] GetPossibleNeighbours(Cell cell)
    {
        int offsetY = -1;
        if (cell.x % 2 == 0)
            offsetY = 1;

        Vector2Int[] result = new Vector2Int[6];
        result[0] = new Vector2Int(cell.x + 0, cell.y - 1);
        result[1] = new Vector2Int(cell.x + 0, cell.y + 1);
        result[2] = new Vector2Int(cell.x - 1, cell.y + 0);
        result[3] = new Vector2Int(cell.x + 1, cell.y + 0);
        result[4] = new Vector2Int(cell.x - 1, cell.y + offsetY);
        result[5] = new Vector2Int(cell.x + 1, cell.y + offsetY);

        return result;
    }
    #endregion
}
