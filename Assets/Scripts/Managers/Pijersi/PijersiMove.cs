public partial class Pijersi
{
    private void OnEnterMove()
    {
        canMove = false;
        if (!selectedCell.isFull)
            canStack = false;
        ActionType action = pointedCell.isEmpty ? ActionType.Move : ActionType.Attack;
        board.KillPieces(pointedCell);
        board.Move(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action);
    }

    private void OnExitMove() { }

    private void OnUpdateMove()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            SM.ChangeState(State.End);
            return;
        }

        // prochaine action
        if (CheckReplayState()) return;

        if (canStack && pointedCell.isFull)
        {
            NextActionState();
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
