using System.Collections;
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

    public PieceType type;
    public PieceType prey;
    [HideInInspector] public int team;
    [HideInInspector] public Cell cell;

    private Vector3 startPosition;
    private Quaternion startRotation;
    public Vector3 endPosition { get; private set; }
    private Quaternion endRotation;
    private float startTime;
    private bool isJump;

    private void Awake()
    {
        transform = base.transform;
    }

    public void InitMove(Cell cell, float rng, float y = float.NaN)
    {
        this.cell = cell;
        isJump = !float.IsNaN(y);

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].endPosition : cell.transform.position;

        Vector2 randPosition = Random.insideUnitCircle * maxPositionOffset * rng;
        Vector3 positionOffset = new Vector3(randPosition.x, isJump ? y : transform.position.y, randPosition.y);
        float angleOffset = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        startPosition = transform.position;
        startRotation = transform.rotation;
        endPosition = position + positionOffset;
        endRotation = Quaternion.Euler(Vector3.up * angleOffset * rng);
        startTime = Time.time;
    }

    public bool UptadeMove()
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

    public void MoveTo(Cell cell, float rng, float y = 0f)
    {
        this.cell = cell;

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].transform.position : cell.transform.position;

        Vector2 randPosition    = Random.insideUnitCircle * maxPositionOffset * rng;
        Vector3 positionOffset  = new Vector3(randPosition.x, y, randPosition.y);
        float angleOffset       = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        transform.position = position + positionOffset;
        transform.rotation = Quaternion.Euler(Vector3.up * angleOffset * rng);
        endPosition = transform.position;
    }

    public virtual Dictionary<Cell, List<ActionType>> GetValidMoves(bool canMove, bool canStack)
    {
        Dictionary<Cell, List<ActionType>> validMoves = new Dictionary<Cell, List<ActionType>>();

        foreach (Cell near in cell.nears)
        {
            if (near == null) continue;

            List<ActionType> validActions = new List<ActionType>();

            if (canMove)
            {
                if (near.isEmpty)
                    validActions.Add(ActionType.Move);
                else if (near.pieces[0].team != team && near.lastPiece.type == prey)
                    validActions.Add(ActionType.Attack);
            }

            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team))
                    validActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team)
                    validActions.Add(ActionType.Stack);
            }

            if (validActions.Count > 0)
                validMoves.Add(near, validActions);
        }

        if (!canMove)
            return validMoves;

        foreach (Cell farNear in cell.GetFarNears())
        {
            if (farNear == null) continue;

            List<ActionType> validActions = new List<ActionType>();

            if (farNear.isEmpty)
                validActions.Add(ActionType.Move);
            else if (farNear.pieces[0].team != team && farNear.lastPiece.type == prey)
                validActions.Add(ActionType.Attack);

            if (validActions.Count > 0)
                validMoves.Add(farNear, validActions);
        }

        return validMoves;
    }
}
