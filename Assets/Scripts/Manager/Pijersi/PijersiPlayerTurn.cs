using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private void OnEnterPlayerTurn() { }
    private void OnExitPlayerTurn() { }

    private void OnUpdatePlayerTurn()
    {
        if (!CheckPointedCell()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && pointedCell.pieces[0]?.team == currentTeamId)
        {
            SM.ChangeState(State.Selection);
            return;
        }

        animation.UpdateHighlight(pointedCell);
    }
}
