using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wise : Piece
{
    public override Dictionary<Cell, List<ActionType>> GetValideMoves(bool canMove, bool canStack)
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
            }

            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team))
                    valideActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team && near.pieces[0].type == PieceType.Wise)
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
