using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterAiTurn()
    {
        aiActionStates  = new State[2];
        aiActionCells   = new Cell[3];
        playAuto        = null;
        StartCoroutine(PlayAuto());
    }

    private void OnExitAiTurn() { }

    private void OnUpdateAiTurn()
    {
        if (playAuto == null) return;

        aiActionCells[0] = board.cells[board.CoordsToIndex(playAuto[0], playAuto[1])];
        if (playAuto[2] > -1)
            aiActionCells[1] = board.cells[board.CoordsToIndex(playAuto[2], playAuto[3])];
        aiActionCells[2] = board.cells[board.CoordsToIndex(playAuto[4], playAuto[5])];

        // actions simples
        if (aiActionCells[1] == null) // move
        {
            canStack = false;
            aiActionStates = new State[] { State.Move };
            aiActionCells = new Cell[] { aiActionCells[0], aiActionCells[2] };

            SM.ChangeState(State.PlayAuto);
            return;
        }

        if (aiActionCells[1] == aiActionCells[0]) // (un)stack
        {
            canMove = false;
            State newState = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team ? State.Stack : State.Unstack;
            aiActionStates = new State[] { newState };
            aiActionCells = new Cell[] { aiActionCells[0], aiActionCells[2] };

            SM.ChangeState(State.PlayAuto);
            return;
        }

        // actions composées
        if (aiActionCells[1].pieces[0]?.team != aiActionCells[0].pieces[0].team) // move -> (un)stack
        {
            aiActionStates[1] = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team ? State.Stack : State.Unstack;
            aiActionStates[0] = State.Move;

            SM.ChangeState(State.PlayAuto);
            return;
        }

        aiActionStates[1] = State.Move;
        aiActionStates[0] = State.Stack;

        SM.ChangeState(State.PlayAuto);
    }

    IEnumerator PlayAuto()
    {
        playAuto = engine.PlayAuto(2);

        yield return null;
    }
}
