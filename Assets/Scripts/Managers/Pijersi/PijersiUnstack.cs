public partial class Pijersi
{
    private void OnEnterUnstack()
    {
        canStack = false;
        canMove = false;
        ActionType action = pointedCell.IsEmpty ? ActionType.Unstack : ActionType.Attack;
        board.KillPieces(pointedCell);
        board.Unstack(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, action);
    }

    private void OnUpdateUnstack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            SM.ChangeState(State.End);
            return;
        }

        if (TryReplayState()) return;

        SM.ChangeState(State.Turn);
    }
}
