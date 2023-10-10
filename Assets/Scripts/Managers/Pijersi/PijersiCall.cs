using UnityEngine;

public partial class Pijersi
{
    public void ResetMatch()
    {
        if (config.partyData != null)
        {
            save = new(loadedSave);
            config.playerTypes = save.playerTypes;
            Replay();

            return;
        }

        InitEngine();
        save = new Save(new PlayerType[] { teams[0].type, teams[1].type });
        teams[0].score = 0;
        teams[1].score = 0;
        replayState = ReplayState.None;
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();
        cameraMovement.ResetPosition();

        SM.ChangeState(State.Turn);
    }

    public void TogglePause()
    {
        isPauseOn = !isPauseOn;
        Time.timeScale = 1 - Time.timeScale;
    }

    public void Replay()
    {
        replaySave = new Save(save);
        save = new Save(new PlayerType[] { teams[0].type, teams[1].type });
        replayState = ReplayState.Play;
        teams[currentTeamId].score = Mathf.Min(0, CurrentTeam.score - 1);
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI(teams[0].score, teams[1].score);
        UI.ReplayButtons["Play"].interactable = true;
        cameraMovement.ResetPosition();

        SM.ChangeState(State.Turn);
    }

    public void PausePlay()
    {
        replayState = replayState == ReplayState.Play ? ReplayState.Pause : ReplayState.Play;
        if (SM.CurrentState.Id == State.PlayerTurn)
            SM.ChangeState(State.Replay);
    }

    public void Back(bool isTurn)
    {
        replayState = ReplayState.Pause;

        replayTo.turnId = save.turns.Count - 1;
        if (save.turns[replayTo.turnId].actions.Count == 0)
            replayTo.turnId--;

        replayTo.actionId = isTurn ? 0 : save.turns[replayTo.turnId].actions.Count - 1;

        UI.ReplayButtons["Back"].interactable = false;
        UI.ReplayButtons["Play"].interactable = true;
        UI.ReplayButtons["Next"].interactable = true;

        SM.ChangeState(State.Back);
    }
    
    public void Next(bool isTurn)
    {
        replayTo.turnId = save.turns.Count - 1;
        replayTo.actionId = isTurn ? replaySave.turns[replayTo.turnId].actions.Count - 1 : save.turns[replayTo.turnId].actions.Count;

        UI.ReplayButtons["Back"].interactable = true;
        if (replayTo.turnId == replaySave.turns.Count - 1 && replayTo.actionId == replaySave.turns[replayTo.turnId].actions.Count - 1)
            UI.ReplayButtons["Play"].interactable = false;
        UI.ReplayButtons["Next"].interactable = false;

        SM.ChangeState(State.Next);
    }

    public void Save() => save.Write();
    public void AfterReplay() => SM.ChangeState(State.Turn);
}
