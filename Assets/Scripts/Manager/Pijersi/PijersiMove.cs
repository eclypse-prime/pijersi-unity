using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterMove()
    {
        canMove = false;
        ActionType action = pointedCell.isEmpty ? ActionType.Move : ActionType.Attack;
        board.Move(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action);
    }

    private void OnExitMove() { }

    private void OnUpdateMove()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            SM.ChangeState(State.End);
            return;
        }

        // prochaine action
        if (canStack && pointedCell.isFull)
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
