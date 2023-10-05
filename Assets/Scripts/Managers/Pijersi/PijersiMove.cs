public partial class Pijersi
{
    private void OnEnterMove()
    {
        canMove = false;
        if (!selectedCell.IsFull)
            canStack = false;
        board.KillPieces(pointedCell);
        board.Move(selectedCell, pointedCell);
        save.AddAction(currentAction, selectedCell, pointedCell);
        UI.UpdateRecord(currentTeamId, selectedCell, pointedCell, currentAction);
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
