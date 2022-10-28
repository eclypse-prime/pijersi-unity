using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private void OnEnterPlayerTurn()
    {
        if (cameraMovement.position != CameraMovement.positionType.Up)
            cameraMovement.position = currentTeamId == 0 ? CameraMovement.positionType.White : CameraMovement.positionType.Black;
    }
    private void OnExitPlayerTurn() { }

    private void OnUpdatePlayerTurn()
    {
        CheckCamera();

        if (!CheckPointedCell()) return;

        if ((Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) && pointedCell.pieces[0]?.team == currentTeamId)
        {
            SM.ChangeState(State.Selection);
            return;
        }

        animation.UpdateHighlight(pointedCell);
    }
}
