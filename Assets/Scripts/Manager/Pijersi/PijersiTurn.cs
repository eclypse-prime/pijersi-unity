using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterTurn()
    {
        currentTeamId = 1 - currentTeamId;
        canMove       = true;
        canStack      = true;
        selectedCell  = null;
        pointedCell   = null;
        UpdateEngine();
        save.AddTurn();
    }

    private void OnExitTurn()
    {
        UI.UpdateGameState(currentTeamId, teams[currentTeamId].Name);
        UI.AddRecordColumnLine(currentTeamId);

        if (replaySave != null)
            UI.replayButtons["Next"].interactable = true;
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

    private void UpdateEngine()
    {
        if (save.turns.Count == 0 || config.playerTypes[currentTeamId] == PlayerType.Human || config.playerTypes[1 - currentTeamId] != PlayerType.Human) return;

        if (engine == null)
        {
            engine = new Engine();
            engine.SetState(board.GetState());
            engine.SetPlayer((byte)currentTeamId);
        }

        int[] manualPlay = new int[6];
        Save.Turn lastTurn = save.turns[save.turns.Count - 1];
        manualPlay[0] = lastTurn.cells[0].x;
        manualPlay[1] = lastTurn.cells[0].y;
        // actions simples
        if (lastTurn.actions.Count < 2)
        {
            if (lastTurn.actions[0] == ActionType.Unstack || lastTurn.actions[0] == ActionType.Stack) // (un)stack
            {
                manualPlay[2] = lastTurn.cells[0].x;
                manualPlay[3] = lastTurn.cells[0].y;
            }
            else // move
            {
                manualPlay[2] = -1;
                manualPlay[3] = -1;
            }
            manualPlay[4] = lastTurn.cells[1].x;
            manualPlay[5] = lastTurn.cells[1].y;

            engine.PlayManual(manualPlay);
            return;
        }

        manualPlay[2] = lastTurn.cells[1].x;
        manualPlay[3] = lastTurn.cells[1].y;
        manualPlay[4] = lastTurn.cells[2].x;
        manualPlay[5] = lastTurn.cells[2].y;

        engine.PlayManual(manualPlay);
    }
}
