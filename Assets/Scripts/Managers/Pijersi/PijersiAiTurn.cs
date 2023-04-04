using System.Collections;

public partial class Pijersi
{
    private void OnEnterAiTurn()
    {
        aiActionStates  = new State[2];
        aiActionCells   = new Cell[3];
        playAuto        = null;
        StartCoroutine(PlayAuto());
    }

    private void OnExitAiTurn() {}

    private void OnUpdateAiTurn()
    {
        if (playAuto == null) return;

        UI.replayButtons["Back"].interactable = true;
        UI.replayButtons["Play"].interactable = false;
        UI.replayButtons["Next"].interactable = false;

        aiActionCells[0] = board.cells[playAuto[0]];
        // 0xFFU represents a null action
        if (playAuto[1] != 0xFFU)
            aiActionCells[1] = board.cells[playAuto[1]];
        aiActionCells[2] = board.cells[playAuto[2]];

        // simple action
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

        // compound action
        if (aiActionCells[1].pieces[0]?.team != aiActionCells[0].pieces[0].team) // move -> (un)stack
        {
            aiActionStates[0] = State.Move;
            aiActionStates[1] = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team && aiActionCells[2] != aiActionCells[0] ? State.Stack : State.Unstack;
            SM.ChangeState(State.PlayAuto);
            return;
        }

        aiActionStates[0] = State.Stack;
        aiActionStates[1] = State.Move;

        SM.ChangeState(State.PlayAuto);
    }

    IEnumerator PlayAuto()
    {
        playAuto = engine.PlayAuto((int) config.playerTypes[currentTeamId]);

        yield return null;
    }
}
