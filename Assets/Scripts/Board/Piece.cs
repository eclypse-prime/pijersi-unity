using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
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

    public virtual Dictionary<Cell, List<ActionType>> GetValideMoves(bool canMove, bool canStack)
    {
        Dictionary<Cell, List<ActionType>> valideMoves = new Dictionary<Cell, List<ActionType>>();

        foreach (Cell near in cell.nears)
        {
            if (near == null) continue;

            List<ActionType> valideActions = new List<ActionType>();

            if (canMove)
            {
                if (near.isEmpty)
                    valideActions.Add(ActionType.Move);
                else if (near.pieces[0].team != team && near.lastPiece.type == prey)
                    valideActions.Add(ActionType.Attack);
            }

            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team))
                    valideActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team)
                    valideActions.Add(ActionType.Stack);
            }

            if (valideActions.Count > 0)
                valideMoves.Add(near, valideActions);
        }

        if (!canMove)
            return valideMoves;

        foreach (Cell farNear in cell.GetFarNears())
        {
            if (farNear == null) continue;

            List<ActionType> valideActions = new List<ActionType>();

            if (farNear.isEmpty)
                valideActions.Add(ActionType.Move);
            else if (farNear.pieces[0].team != team && farNear.lastPiece.type == prey)
                valideActions.Add(ActionType.Attack);

            if (valideActions.Count > 0)
                valideMoves.Add(farNear, valideActions);
        }

        return valideMoves;
    }
}
