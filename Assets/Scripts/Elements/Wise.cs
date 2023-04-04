using System.Collections.Generic;

public class Wise : Piece
{
    public override Dictionary<Cell, List<ActionType>> GetLegalMoves(bool canMove, bool canStack)
    {
        Dictionary<Cell, List<ActionType>> legalMoves = new Dictionary<Cell, List<ActionType>>();

        // get legal moves for nearby cells
        foreach (Cell near in cell.nears)
        {
            if (near == null) continue;

            List<ActionType> legalActions = new List<ActionType>();

            // move/attack
            if (canMove)
            {
                if (near.isEmpty)
                    legalActions.Add(ActionType.Move);
            }

            // unstack/stack
            if (canStack)
            {
                if (cell.isFull && (near.isEmpty || near.pieces[0].team != team))
                    legalActions.Add(ActionType.Unstack);
                else if (!near.isEmpty && !near.isFull && near.pieces[0].team == team && near.pieces[0].type == PieceType.Wise)
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

            // move
            if (farNear.isEmpty)
                legalActions.Add(ActionType.Move);

            if (legalActions.Count > 0)
                legalMoves.Add(farNear, legalActions);
        }

        return legalMoves;
    }

    public override Cell[] GetDangers(Piece[] pieces, Cell cell)
    {
        return null;
    }

    public override Dictionary<Cell, Cell[]> GetDangers(Piece[] pieces, Cell[] cells)
    {
        return null;
    }

    protected override bool CanAttack(Piece targetPiece, Cell targetCell)
    {
        return false;
    }
}
