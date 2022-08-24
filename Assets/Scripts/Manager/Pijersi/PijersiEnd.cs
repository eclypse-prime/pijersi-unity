using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterEnd()
    {
        playerScores[currentTeamId]++;
        UI.ShowEnd(currentTeamId, playerScores, config.winMax);
        TogglePause();
    }

    private void OnExitEnd()
    {
        if (config.playerTypes[0] != PlayerType.Human || config.playerTypes[1] != PlayerType.Human)
            engine = new Engine();
        save = new Save(config.playerTypes);

        // inversion des équipes
        string firstName = playerNames[0];
        playerNames[0] = playerNames[1];
        playerNames[1] = firstName;

        int firstScore = playerScores[0];
        playerScores[0] = playerScores[1];
        playerScores[1] = firstScore;

        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();
    }

    private void OnUpdateEnd()
    {
        if (playerScores[currentTeamId] < config.winMax)
            SM.ChangeState(State.Turn);
    }
}
