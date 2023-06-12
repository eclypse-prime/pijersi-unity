using System.Collections.Generic;

public partial class Piece
{
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
        Dictionary<Cell, List<ActionType>> legalMoves = new();
        GetLegalNearMoves(ref legalMoves, canMove, canStack);

        if (!canMove)
            return legalMoves;

        GetLegalFarMoves(ref legalMoves);

        return legalMoves;
    }

    protected virtual void GetLegalNearMoves(ref Dictionary<Cell, List<ActionType>> legalMoves, bool canMove, bool canStack)
    {
        List<ActionType> legalActions;
        foreach (Cell near in cell.Nears)
        {
            if (near == null) continue;

            legalActions = new();
            GetNearMoveAttack(near, canMove, ref legalActions);
            GetNearUnstackStack(near, canStack, ref legalActions);

            if (legalActions.Count > 0)
                legalMoves.Add(near, legalActions);
        }
    }

    protected virtual void GetNearMoveAttack(Cell near, bool canMove, ref List<ActionType> legalActions)
    {
        if (!canMove) return;

        if (near.IsEmpty)
            legalActions.Add(ActionType.Move);
        else if (near.pieces[0].team != team && near.LastPiece.type == prey)
            legalActions.Add(ActionType.Attack);
    }

    protected virtual void GetNearUnstackStack(Cell near, bool canStack, ref List<ActionType> legalActions)
    {
        if (!canStack) return;

        if (cell.IsFull && (near.IsEmpty || near.pieces[0].team != team && near.LastPiece.type == prey))
            legalActions.Add(ActionType.Unstack);
        else if (!near.IsEmpty && !near.IsFull && near.pieces[0].team == team)
            legalActions.Add(ActionType.Stack);
    }


    protected virtual void GetLegalFarMoves(ref Dictionary<Cell, List<ActionType>> legalMoves)
    {
        List<ActionType> legalActions;
        for (int i = 0; i < 6; i++)
        {
            Cell farNear = cell.Nears[i]?.Nears[i];
            if (farNear == null || !cell.Nears[i].IsEmpty) continue;

            legalActions = new();
            GetfarMoveAttack(farNear, ref legalActions);

            if (legalActions.Count > 0)
                legalMoves.Add(farNear, legalActions);
        }
    }

    protected virtual void GetfarMoveAttack(Cell farNear, ref List<ActionType> legalActions)
    {
        if (farNear.IsEmpty)
            legalActions.Add(ActionType.Move);
        else if (farNear.pieces[0].team != team && farNear.LastPiece.type == prey)
            legalActions.Add(ActionType.Attack);
    }
}