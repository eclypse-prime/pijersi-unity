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
        UI.SetGameState(currentTeamId, teams[currentTeamId].Type, teams[currentTeamId].Number);
        UI.AddRecordColumnLine(currentTeamId);
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
            engine.SetPlayer((byte)(1 - currentTeamId));
        }

        int[] manualPlay = new int[3];
        Save.Turn lastTurn = save.turns[save.turns.Count - 1];
        manualPlay[0] = board.CoordsToIndex(lastTurn.cells[0].x, lastTurn.cells[0].y);

        // actions simples
        if (lastTurn.actions.Count < 2)
        {
            if (lastTurn.actions[0] == ActionType.Unstack || lastTurn.actions[0] == ActionType.Stack) // (un)stack
            {
                manualPlay[1] = board.CoordsToIndex(lastTurn.cells[0].x, lastTurn.cells[0].y);
            }
            else // move
            {
                manualPlay[1] = -1;
            }
            manualPlay[2] = board.CoordsToIndex(lastTurn.cells[2].x, lastTurn.cells[2].y);

            engine.PlayManual(manualPlay);
            return;
        }

        manualPlay[1] = board.CoordsToIndex(lastTurn.cells[1].x, lastTurn.cells[1].y);
        manualPlay[2] = board.CoordsToIndex(lastTurn.cells[2].x, lastTurn.cells[2].y);

        engine.PlayManual(manualPlay);
    }
}
