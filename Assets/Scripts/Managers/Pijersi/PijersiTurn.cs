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
        UI.SetGameState(currentTeamId, teams[currentTeamId].Type, teams[currentTeamId].Number);
        UI.AddRecordColumnLine(currentTeamId);
    }

    private void OnUpdateTurn()
    {
        if (replayState != ReplayState.None)
        {
            SM.ChangeState(State.Replay);
            return;
        }

        if (config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            SM.ChangeState(State.PlayerTurn);
            return;
        }

        SM.ChangeState(State.AiTurn);
    }
}
