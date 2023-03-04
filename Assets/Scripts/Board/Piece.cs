using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Piece : MonoBehaviour
{
    private const int maxRotationOffset = 20;
    private const float maxPositionOffset = .2f;
    private const float moveDuration = 1f;

    [SerializeField] private AnimationCurve jumpCurve;
    public new Transform transform { get; private set; }
    public MeshRenderer mainRenderer;
    public MeshRenderer[] SignRenderer;
    public Vector3 endPosition { get; private set; }

    public PieceType type;
    public PieceType prey;
    [HideInInspector] public int team;
    [HideInInspector] public Cell cell;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float startTime;
    private bool isJump;

    private void Awake()
    {
        transform = base.transform;
    }


    /// <summary>
    /// Init piece's move from his current position to his new cell.
    /// Applies a random offset to the destination position and rotation.
    /// </summary>
    /// <remarks>
    /// Cell.pieces must be updated before using this function.
    /// </remarks>
    /// <param name="cell">The destination cell.</param>
    /// <param name="rngPercent">Percent of rng applied to the destination position and rotation. Value between 0 and 1.</param>
    /// <param name="y">(optional) Y component of the destination position. Used for stack and unstack.</param>
    public void InitMove(Cell cell, float rngPercent, float y = float.NaN)
    {
        this.cell = cell;
        isJump = !float.IsNaN(y);

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].endPosition : cell.transform.position;

        Vector2 randPosition = Random.insideUnitCircle * maxPositionOffset * rngPercent;
        Vector3 positionOffset = new Vector3(randPosition.x, isJump ? y : transform.position.y, randPosition.y);
        float angleOffset = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        startPosition = transform.position;
        startRotation = transform.rotation;
        endPosition = position + positionOffset;
        endRotation = Quaternion.Euler(Vector3.up * angleOffset * rngPercent);
        startTime = Time.time;
    }

    /// <summary>
    /// Update piece move.
    /// </summary>
    /// <returns><list type="table">
    /// <item><term>True</term> the move isn't finished.</item>
    /// <item><term>False</term> the move is finished.</item>
    /// </list></returns>
    public bool UpdateMove()
    {
        if (startTime == 0f) return false;

        float step = Mathf.SmoothStep(0f, 1f, (Time.time - startTime) / moveDuration);
        Vector3 offset = isJump ? Vector3.up * jumpCurve.Evaluate(step) : Vector3.zero;
        transform.position = Vector3.Lerp(startPosition, endPosition, step) + offset;
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, step);

        if (step >= 1f)
        {
            startTime = 0f;
            return false;
        }
        return true;
    }

    /// <summary>
    /// Directly move this piece to the destination cell.
    /// </summary>
    /// <remarks></remarks>
    /// <inheritdoc cref="InitMove(Cell, float, float)"/>
    public void MoveTo(Cell cell, float rngPercent, float y = 0f)
    {
        this.cell = cell;

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].transform.position : cell.transform.position;

        Vector2 randPosition    = Random.insideUnitCircle * maxPositionOffset * rngPercent;
        Vector3 positionOffset  = new Vector3(randPosition.x, y, randPosition.y);
        float angleOffset       = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        transform.position = position + positionOffset;
        transform.rotation = Quaternion.Euler(Vector3.up * angleOffset * rngPercent);
        endPosition = transform.position;
    }

    /// <summary>
    /// Return all possible moves with this piece.
    /// </summary>
    /// <param name="canMove"></param>
    /// <param name="canStack"></param>
    /// <returns>
    /// Dictionary of :
    /// <list type="bullet">
    /// <item>Cell where at least one action can be perform.</item>
    /// <item>Action List that can be perform on the cell.</item>
    /// </list>
    /// </returns>
    public virtual Dictionary<Cell, List<ActionType>> GetLegalMoves(bool canMove, bool canStack)
    {
        Dictionary<Cell, List<ActionType>> legalMoves = new Dictionary<Cell, List<ActionType>>();

        // get leagal moves for nearby cells
        foreach (Cell near in cell.nears)
        {
            if (near == null) continue;

            List<ActionType> legalActions = new List<ActionType>();

            // move/attack
            if (canMove)
            {
                if (near.isEmpty)
                    legalActions.Add(ActionType.Move);
                else if (near.pieces[0].team != team && near.lastPiece.type == prey)
                    legalActions.Add(ActionType.Attack);
            }

            // unstack/stack
            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team && near.lastPiece.type == prey))
                    legalActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team)
                    legalActions.Add(ActionType.Stack);
            }

            if (legalActions.Count > 0)
                legalMoves.Add(near, legalActions);
        }

        if (!canMove)
            return legalMoves;

        // get legal moves for far nearby cells
        for (int i = 0; i < 6; i++)
        {
            Cell farNear = cell.nears[i]?.nears[i];
            if (farNear == null || !cell.nears[i].isEmpty) continue;

            List<ActionType> legalActions = new List<ActionType>();

            // move/attack
            if (farNear.isEmpty)
                legalActions.Add(ActionType.Move);
            else if (farNear.pieces[0].team != team && farNear.lastPiece.type == prey)
                legalActions.Add(ActionType.Attack);

            if (legalActions.Count > 0)
                legalMoves.Add(farNear, legalActions);
        }

        return legalMoves;
    }

    /// <summary>
    /// Returns all cells containing a piece that can eliminate this piece if it is on the designated cell.
    /// </summary>
    /// <param name="cell">A cell where the piece can move through.</param>
    public virtual List<Cell> GetDangers(Cell cell)
    {
        List<Cell> result = new List<Cell>();
        foreach (Cell near in cell.nears)
        {
            if (near == null || result.Contains(near)) continue;

            Piece nearPiece = near.lastPiece;
            if (!near.isEmpty)
            {
                if (nearPiece.team != team && nearPiece.prey == type)
                    result.Add(near);
            }

            foreach (Cell farNear in near.nears)
            {
                if (farNear == null || near.isFull && near.pieces[0]?.team == farNear.pieces[0]?.team || result.Contains(farNear)) continue;

                Piece farPiece = farNear.lastPiece;
                if (!farNear.isEmpty && farPiece.team != team && farPiece.prey == type && (farNear.isFull || near.pieces[0]?.team == farPiece.team))
                    result.Add(farNear);

                foreach (Cell deepNear in farNear.nears)
                {
                    if (deepNear == null || deepNear.isEmpty || farNear.isFull || result.Contains(deepNear)) continue;

                    Piece deepPiece = deepNear.lastPiece;
                    if (deepPiece.team != team && deepPiece.prey == type)
                    {
                        if (deepNear.isFull && farNear.isEmpty && (near.isEmpty || deepPiece.prey == nearPiece.type && deepPiece.team != nearPiece.team))
                            result.Add(deepNear);
                        else if (deepPiece.team == farPiece?.team && near.isEmpty && near.nears[farNear.GetNearIndex(near)] == cell)
                            result.Add(deepNear);
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Returns all cells containing a piece that can eliminate this piece if it is on any designated cells.
    /// </summary>
    /// <param name="cells">Cell list where the piece can move through.</param>
    public virtual Dictionary<Cell, List<Cell>> GetDangers(Cell[] cells)
    {
        Dictionary<Cell, List<Cell>> result = new Dictionary<Cell, List<Cell>>();

        foreach (Cell cell in cells)
        {
            List<Cell> dangers = GetDangers(cell);

            if (dangers.Count > 0)
                result.Add(cell, dangers);
        }

        return result;
    }

    /// <summary>
    /// Return the binary code of this piece.
    /// </summary>
    public ushort ToByte()
    {
        ushort result = 1;
        if (team == 1)
            result += 2;
        switch (type)
        {
            case PieceType.Scissors:
                break;
            case PieceType.Paper:
                result += 4;
                break;
            case PieceType.Rock:
                result += 8;
                break;
            case PieceType.Wise:
                result += 12;
                break;
            default:
                break;
        }

        return result;
    }
}
