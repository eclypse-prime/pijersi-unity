public partial class Pijersi
{
    private void OnEnterAfterReplay()
    {
        replayState = ReplayState.None;
        UI.ShowAfterReplayMenu(teams[0].type, teams[1].type);
    }

    private void OnExitAfterReplay()
    {
        config.playerTypes = UI.GetReplayPlayerTypes();
        InitTeams();
        InitEngine();

        void InitEngine()
        {
            if (CurrentTeam.type == PlayerType.Human) return;

            engine = new Engine();
            engine.SetState(board.GetState());
            engine.SetPlayer((byte)(1-currentTeamId));
            GetNextAiTurn(OtherTeam.type);
        }
    }
}