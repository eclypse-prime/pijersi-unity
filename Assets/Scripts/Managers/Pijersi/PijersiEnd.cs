public partial class Pijersi
{
    private void OnEnterEnd()
    {
        teams[currentTeamId].score++;
        int[] scores = { teams[0].score, teams[1].score };
        UI.ShowEnd(currentTeamId, CurrentTeam.type, CurrentTeam.number, scores, config.winMax);
        TogglePause();
        replayState = ReplayState.None;
    }

    private void OnUpdateEnd()
    {
        InitEngine();
        replayState = ReplayState.None;

        (teams[0], teams[1]) = (teams[1], teams[0]);
        save = new Save(new PlayerType[] { teams[0].type, teams[1].type });
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetOverlay();

        SM.ChangeState(State.Turn);
    }
}
