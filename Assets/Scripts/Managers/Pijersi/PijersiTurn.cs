public partial class Pijersi
{
    private void OnEnterTurn()
    {
        currentTeamId = 1 - currentTeamId;
        canMove = true;
        canStack = true;
        selectedCell = null;
        pointedCell = null;
        save.AddTurn();

        //DebugEngine();
    }

    private void OnExitTurn()
    {
        UI.SetGameState(currentTeamId, CurrentTeam.type, CurrentTeam.number);
        UI.AddRecordColumnLine(currentTeamId);
    }

    private void OnUpdateTurn()
    {
        if (replayState != ReplayState.None)
        {
            SM.ChangeState(State.Replay);
            return;
        }

        if (CurrentTeam.type == PlayerType.Human)
        {
            SM.ChangeState(State.PlayerTurn);
            return;
        }

        SM.ChangeState(State.AiTurn);
    }
}
