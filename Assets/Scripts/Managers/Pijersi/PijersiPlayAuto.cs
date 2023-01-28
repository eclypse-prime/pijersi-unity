using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    private void OnEnterPlayAuto() { }
    private void OnExitPlayAuto() { }

    private void OnUpdatePlayAuto()
    {
        selectedCell = aiActionCells[0];
        pointedCell  = aiActionCells[1];

        SM.ChangeState(aiActionStates[0]);

        if (aiActionStates.Length < 2) return;

        // supprime l'action en cours
        aiActionCells  = new Cell[] { aiActionCells[1], aiActionCells[2] };
        aiActionStates = new State[] { aiActionStates[1] };
    }
}
