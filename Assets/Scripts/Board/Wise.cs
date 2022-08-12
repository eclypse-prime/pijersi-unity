using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wise : Piece
{
    public override Dictionary<Cell, List<ActionType>> GetValidMoves(bool canMove, bool canStack)
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
            }

            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team))
                    validActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team && near.pieces[0].type == PieceType.Wise)
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
