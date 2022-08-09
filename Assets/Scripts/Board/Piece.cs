using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public new Transform transform { get; private set; }
    public MeshRenderer mainRenderer;
    public MeshRenderer[] SignRenderer;

    public PieceType type;
    public PieceType prey;
    [HideInInspector] public int team;
    [HideInInspector] public Cell cell;

    private void Awake()
    {
        transform = base.transform;
    }

    public void MoveTo(Cell cell, float y = 0f)
    {
        this.cell = cell;
        transform.position = cell.transform.position + Vector3.up * y;
    }

    public virtual Dictionary<ActionType, List<Cell>> GetValideMoves(Cell[] cells, bool canMove, bool canStack)
    {
        Dictionary<ActionType, List<Cell>> result = new Dictionary<ActionType, List<Cell>>();
        result.Add(ActionType.move, new List<Cell>());
        result.Add(ActionType.attack, new List<Cell>());
        result.Add(ActionType.stack, new List<Cell>());
        result.Add(ActionType.unstack, new List<Cell>());

        foreach (Cell target in cells)
        {
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

        return result;
    }
}
