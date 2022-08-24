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
    }

    private void OnExitStack() { }

    private void OnUpdateStack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (canMove)
        {
            if (config.playerTypes[currentTeamId] != PlayerType.Human)
            {
                SM.ChangeState(State.PlayAuto);
                return;
            }

            SM.ChangeState(State.Selection);
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
