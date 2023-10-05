public partial class Pijersi
{
    private void OnEnterUnstack()
    {
        canStack = false;
        canMove = false;
        board.KillPieces(pointedCell);
        board.Unstack(selectedCell, pointedCell);
        save.AddAction(currentAction, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, currentAction);
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
