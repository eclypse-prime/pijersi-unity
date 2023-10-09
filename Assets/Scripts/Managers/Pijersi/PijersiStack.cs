public partial class Pijersi
{
    private void OnEnterStack()
    {
        canStack = false;
        board.Stack(selectedCell, pointedCell);
        save.AddAction(currentAction, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, currentAction);
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
