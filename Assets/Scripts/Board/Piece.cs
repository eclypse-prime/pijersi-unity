using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private const int maxRotationOffset = 20;
    private const float maxPositionOffset = .1f;

    public new Transform transform { get; private set; }
    public MeshRenderer mainRenderer;
    public MeshRenderer[] SignRenderer;

    public PieceType type;
    public PieceType prey;
    [HideInInspector] public int team;
    [HideInInspector] public Cell cell;

    private Quaternion baseRotation;

    private void Awake()
    {
        transform = base.transform;
        baseRotation = transform.rotation;
    }

    public void MoveTo(Cell cell, float stepX, float rng, float y = 0f)
    {
        this.cell = cell;

        Vector3 position = cell.pieces[0] != this ? cell.pieces[0].transform.position : cell.transform.position;

        Vector2 randPosition    = Random.insideUnitCircle * stepX * maxPositionOffset * rng;
        Vector3 positionOffset  = new Vector3(randPosition.x, 0f, randPosition.y);
        float angleOffset       = Mathf.Lerp(-maxRotationOffset, maxRotationOffset, Random.value);

        transform.position = position + Vector3.up * y + positionOffset;
        transform.rotation = Quaternion.Euler(baseRotation.eulerAngles + Vector3.up * angleOffset * rng);
    }

    public virtual Dictionary<ActionType, List<Cell>> GetValideMoves(bool canMove, bool canStack)
    {
        Dictionary<ActionType, List<Cell>> result = new Dictionary<ActionType, List<Cell>>();
        result.Add(ActionType.move, new List<Cell>());
        result.Add(ActionType.attack, new List<Cell>());
        result.Add(ActionType.stack, new List<Cell>());
        result.Add(ActionType.unstack, new List<Cell>());

        foreach (Cell target in cell.nears)
        {
            if (target == null) continue;

            if (canMove)
            {
                if (target.isEmpty)
                    result[ActionType.move].Add(target);
                else if (target.pieces[0].team != team && target.lastPiece.type == prey)
                {
                    result[ActionType.attack].Add(target);
                }
            }

            if (canStack)
            {
                if (cell.isFull && (target.isEmpty || target.pieces[0].team != team))
                    result[ActionType.unstack].Add(target);
                else if (!target.isEmpty && !target.isFull && target.pieces[0].team == team)
                    result[ActionType.stack].Add(target);
            }
        }

        if (!canMove)
            return result;

        foreach (Cell target in cell.GetFarNears())
        {
            if (target == null) continue;

            if (target.isEmpty)
                result[ActionType.move].Add(target);
            else if (target.pieces[0].team != team && target.lastPiece.type == prey)
            {
                result[ActionType.attack].Add(target);
            }
        }

        return result;
    }
}
