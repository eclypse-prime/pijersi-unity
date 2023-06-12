public partial class Pijersi
{
    private void OnEnterMove()
    {
        canMove = false;
        if (!selectedCell.IsFull)
            canStack = false;
        ActionType action = pointedCell.IsEmpty ? ActionType.Move : ActionType.Attack;
        board.KillPieces(pointedCell);
        board.Move(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, action);
    }

    private void OnUpdateMove()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            SM.ChangeState(State.End);
            return;
        }

        if (TryReplayState()) return;

        if (canStack && pointedCell.IsFull)
        {
            NextActionState();
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
