using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private bool CheckPointedCell()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out hit, 50f, cellLayer))
        {
            pointedCell = null;
            return false;
        }

        pointedCell = hit.transform.GetComponentInParent<Cell>();
        return true;
    }

    private bool CheckPause()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();

        return isPauseOn;
    }

    public void ResetMatch(bool isStart)
    {
        if (config.playerTypes[0] != PlayerType.Human || config.playerTypes[1] != PlayerType.Human)
            engine = new Engine();
        save = new Save(config.playerTypes);

        // reset des noms d'équipe
        playerNames = new string[2];
        playerNames[0] = config.playerTypes[0] == PlayerType.Human ? "Player" : "AI";
        playerNames[1] = config.playerTypes[1] == PlayerType.Human ? "Player" : "AI";

        if (config.playerTypes[0] == config.playerTypes[1])
        {
            playerNames[0] += " #1";
            playerNames[1] += " #2";
        }

        playerScores = new int[2];
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();

        SM.ChangeState(State.Turn);
    }

    private bool IsWin(Cell cell)
    {
        if (cell.lastPiece.type == PieceType.Wise) return false;
        if (cell.x == 0 && cell.pieces[0].team == 0 || cell.x == board.LineCount - 1 && cell.pieces[0].team == 1)
            return true;

        return false;
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

    IEnumerator PlayAuto()
    {
        playAuto = engine.PlayAuto(2);

        yield return null;
    }

    private void AddManualPlay()
    {
        if (config.playerTypes[currentTeamId] != PlayerType.Human || config.playerTypes[1 - currentTeamId] == PlayerType.Human) return;

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

        string text = "";
        foreach (int index in manualPlay)
            text += index;

        engine.PlayManual(manualPlay);
    }
}
