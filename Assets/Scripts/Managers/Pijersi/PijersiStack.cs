public partial class Pijersi
{
    private void OnEnterStack()
    {
        canStack = false;
        board.Stack(selectedCell, pointedCell);
        save.AddAction(ActionType.Stack, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, ActionType.Stack);
    }

    private void OnUpdateStack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (TryReplayState()) return;

        if (canMove)
        {
            NextActionState();
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
