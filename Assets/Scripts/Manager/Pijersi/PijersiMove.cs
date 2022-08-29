using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterMove()
    {
        canMove = false;
        ActionType action = pointedCell.isEmpty ? ActionType.Move : ActionType.Attack;
        board.KillPieces(pointedCell);
        board.Move(selectedCell, pointedCell);
        save.AddAction(ActionType.Move, selectedCell, pointedCell);
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
            ToNextActionState();
            return;
        }

        SM.ChangeState(State.Turn);
    }
}
