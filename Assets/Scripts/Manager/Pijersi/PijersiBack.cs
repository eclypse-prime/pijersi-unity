using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterBack()
    {
        engine = null;
        replaySave ??= new Save(save);

        int turnId = save.turns.Count - 1;
        if (save.turns[turnId].actions.Count == 0)
        {
            save.turns.RemoveAt(turnId);
            currentTeamId = 1 - currentTeamId;
            UI.UpdateGameState(currentTeamId, playerNames[currentTeamId]);
            if (cameraMovement.position != CameraMovement.positionType.Up && config.playerTypes[currentTeamId] == PlayerType.Human)
                cameraMovement.position = currentTeamId == 0 ? CameraMovement.positionType.White : CameraMovement.positionType.Black;
            turnId--;
        }
        Save.Turn turn = save.turns[turnId];
        int actionId = turn.actions.Count - 1;

        selectedCell = turn.cells[actionId + 1];
        pointedCell = turn.cells[actionId];

        switch (turn.actions[actionId])
        {
            case ActionType.Move:
                board.Move(selectedCell, pointedCell);
                canMove = true;
                break;
            case ActionType.Stack:
                board.Unstack(selectedCell, pointedCell);
                canStack = true;
                break;
            case ActionType.Unstack:
                board.Stack(selectedCell, pointedCell);
                canStack = true;
                break;
            default:
                break;
        }
        
        turn.actions.RemoveAt(actionId);
        turn.cells.RemoveAt(actionId + 1);
        UI.UndoRecord();
        UI.replayButtons["Back"].interactable = false;
        UI.replayButtons["Next"].interactable = true;
        if (replayState != ReplayState.None)
            UI.replayButtons["Play"].interactable = true;
    }

    private void OnUpdateBack()
    {
        if (board.UpdateMove(pointedCell)) return;

        board.ReviveLastPieces(selectedCell);

        int turnId = save.turns.Count - 1;
        int actionId = save.turns[turnId].actions.Count - 1;

        if (replayAt.Item1 <= turnId && replayAt.Item2 <= actionId)
        {
            SM.ChangeState(State.Back);
            return;
        }

        replayAt = (-1, -1);
        save.turns[turnId].cells.RemoveAt(0);

        if (turnId > 0 || actionId > 0)
            UI.replayButtons["Back"].interactable = true;

        if (config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            SM.ChangeState(State.PlayerTurn);
            return;
        }

        SM.ChangeState(State.Replay);
    }
}
