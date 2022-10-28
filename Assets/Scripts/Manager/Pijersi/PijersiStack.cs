using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterStack()
    {
        canStack = false;
        board.Stack(selectedCell, pointedCell);
        save.AddAction(ActionType.Stack, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Stack);
        UI.replayButtons["Back"].interactable = true;
    }

    private void OnExitStack() { }

    private void OnUpdateStack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (CheckReplayState()) return;

        if (canMove)
        {
            NextActionState();
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
