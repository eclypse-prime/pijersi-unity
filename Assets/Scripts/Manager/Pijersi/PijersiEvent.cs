using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    public void ResetMatch()
    {
        if (config.playerTypes[0] != PlayerType.Human || config.playerTypes[1] != PlayerType.Human)
            engine = new Engine();
        save = new Save(config.playerTypes);

        // reset des noms d'équipe
        playerNames = new string[2];
        for (int i = 0; i < 2; i++)
        {
            switch (config.playerTypes[i])
            {
                case PlayerType.Human:
                    playerNames[i] = "Player";
                    break;
                case PlayerType.AiEasy:
                    playerNames[i] = "AI (easy)";
                    break;
                case PlayerType.AiNormal:
                    playerNames[i] = "AI (normal)";
                    break;
                case PlayerType.AiHard:
                    playerNames[i] = "AI (hard)";
                    break;
                default:
                    break;
            }
        }

        if (config.playerTypes[0] == config.playerTypes[1])
        {
            playerNames[0] += " #1";
            playerNames[1] += " #2";
        }

        playerScores  = new int[2];
        replayState   = ReplayState.None;
        replayType    = ReplayType.Action;
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();
        cameraMovement.position = CameraMovement.positionType.White;

        SM.ChangeState(State.Turn);
    }

    public void TogglePause()
    {
        isPauseOn = !isPauseOn;
        Time.timeScale = 1 - Time.timeScale;
        UI.SetActivePause(isPauseOn);
    }

    public void Save()
    {
        save.Write();
    }

    public void Replay()
    {
        replaySave = new Save(this.save);
        ResetMatch();
        replayState = ReplayState.Play;
        UI.replayButtons["Play"].interactable = true;
    }

    public void PausePlay()
    {
        replayState = replayState == ReplayState.Play ? ReplayState.Pause : ReplayState.Play;
    }

    public void Back(bool isTurn)
    {
        if (replayState == ReplayState.Play)
            replayState = ReplayState.Pause;

        replayType = isTurn ? ReplayType.Turn : ReplayType.Action;

        UI.SetReplayButtonsInteractable(false);
        SM.ChangeState(State.Back);
    }
    
    public void Next(bool isTurn)
    {
        if (replayState == ReplayState.Play)
            replayState = ReplayState.Pause;

        replayType = isTurn ? ReplayType.Turn : ReplayType.Action;
        UI.SetReplayButtonsInteractable(false);

        if (replayState == ReplayState.Pause && save.turns[save.turns.Count - 1].actions.Count > 0)
            SM.ChangeState(State.Turn);
        SM.ChangeState(State.Next);
    }
}
