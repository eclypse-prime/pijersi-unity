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
            turnId--;
            currentTeamId = 1 - currentTeamId;
            if (save.turns[turnId].actions.Count > 1)
            {
                canMove = false;
                canStack = false;
            }
            else
            {
                canMove = true;
                canStack = true;
            }
            UI.SetGameState(currentTeamId, CurrentTeam.Type, CurrentTeam.Number);
            cameraMovement.position = currentTeamId == 0 ? CameraMovement.positionType.White : CameraMovement.positionType.Black;
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
        if (actionId == 0)
            turn.cells.RemoveAt(actionId);

        UI.UndoRecord(currentTeamId, actionId == 0);
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

        if (turnId > 0 || actionId > -1)
            UI.replayButtons["Back"].interactable = true;

        if (config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            if (actionId == -1)
            {
                SM.ChangeState(State.PlayerTurn);
                return;
            }

            SM.ChangeState(State.Selection);
            return;
        }

        SM.ChangeState(State.Replay);
    }
}
