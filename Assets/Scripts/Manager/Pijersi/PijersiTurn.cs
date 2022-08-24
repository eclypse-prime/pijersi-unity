using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterTurn()
    {
        AddManualPlay();

        currentTeamId = 1 - currentTeamId;
        canMove = true;
        canStack = true;
        selectedCell = null;
        pointedCell = null;
        save.AddTurn();
    }

    private void OnExitTurn()
    {
        UI.UpdateGameState(currentTeamId, playerNames[currentTeamId]);
        UI.AddRecordColumnLine(currentTeamId);
    }

    private void OnUpdateTurn()
    {
        if (config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            SM.ChangeState(State.PlayerTurn);
            return;
        }

        SM.ChangeState(State.AiTurn);
    }
}
