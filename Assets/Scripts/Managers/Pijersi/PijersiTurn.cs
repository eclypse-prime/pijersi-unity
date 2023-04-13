public partial class Pijersi
{
    private void OnEnterTurn()
    {
        currentTeamId = 1 - currentTeamId;
        canMove       = true;
        canStack      = true;
        selectedCell  = null;
        pointedCell   = null;
        save.AddTurn();
    }

    private void OnExitTurn()
    {
        UI.SetGameState(currentTeamId, CurrentTeam.Type, CurrentTeam.Number);
        UI.AddRecordColumnLine(currentTeamId);
    }

    private void OnUpdateTurn()
    {
        if (replayState != ReplayState.None)
        {
            SM.ChangeState(State.Replay);
            return;
        }

        if (CurrentTeam.Type == PlayerType.Human)
        {
            SM.ChangeState(State.PlayerTurn);
            return;
        }

        SM.ChangeState(State.AiTurn);
    }
}
