using System.Collections.Generic;

public class Wise : Piece
{
    protected override void GetNearMoveAttack(Cell near, bool canMove, ref List<ActionType> legalActions)
    {
        if (!canMove || !near.IsEmpty) return;

        legalActions.Add(ActionType.Move);
    }

    protected override void GetNearUnstackStack(Cell near, bool canStack, ref List<ActionType> legalActions)
    {
        if (!canStack) return;

        if (cell.IsFull && near.IsEmpty)
            legalActions.Add(ActionType.Unstack);
        else if (!near.IsEmpty && !near.IsFull && near.pieces[0].team == team && near.pieces[0].type == PieceType.Wise)
            legalActions.Add(ActionType.Stack);
    }

    protected override void GetfarMoveAttack(Cell farNear, ref List<ActionType> legalActions)
    {
        if (!farNear.IsEmpty) return;
        
        legalActions.Add(ActionType.Move);
    }

    public override Cell[] GetDangers(Piece[] pieces, Cell cell) => null;
    public override Dictionary<Cell, Cell[]> GetDangers(Piece[] pieces, Cell[] cells) => null;
    protected override bool CanAttack(Piece targetPiece, Cell targetCell) => false;
}
