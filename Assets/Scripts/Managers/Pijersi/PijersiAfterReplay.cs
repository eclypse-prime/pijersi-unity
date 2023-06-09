public partial class Pijersi
{
    private void OnEnterAfterReplay()
    {
        replayState = ReplayState.None;
        UI.ShowAfterReplayMenu(teams[0].Type, teams[1].Type);
    }

    private void OnExitAfterReplay()
    {
        config.playerTypes = UI.GetReplayPlayerTypes();
        InitTeams();

        if (CurrentTeam.Type != PlayerType.Human)
        {
            engine = new Engine();
            engine.SetState(board.GetState());
            engine.SetPlayer((byte)(1 - currentTeamId));
            GetNextAiTurn(CurrentTeam.Type);
        }
    }
}