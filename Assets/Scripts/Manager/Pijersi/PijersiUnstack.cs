using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterUnstack()
    {
        canStack = false;
        ActionType action = pointedCell.isEmpty ? ActionType.Unstack : ActionType.Attack;
        board.Unstack(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action);
        canMove = false;
    }

    private void OnExitUnstack() { }

    private void OnUpdateUnstack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            SM.ChangeState(State.End);
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
