using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterReplay()
    {
        int turnId     = save.turns.Count - 1;
        int actionId   = save.turns[turnId].actions.Count;
        Save.Turn turn = replaySave.turns[turnId];

        selectedCell = turn.cells[actionId];
        pointedCell  = turn.cells[actionId + 1];

        switch (turn.actions[actionId])
        {
            case ActionType.Move:
                SM.ChangeState(State.Move);
                break;
            case ActionType.Stack:
                SM.ChangeState(State.Stack);
                break;
            case ActionType.Unstack:
                SM.ChangeState(State.Unstack);
                break;
            default:
                break;
        }
    }
    private void OnUpdateReplay() { }
}
