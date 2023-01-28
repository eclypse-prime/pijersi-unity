using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private bool CheckPointedCell()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out hit, 50f, cellLayer))
        {
            pointedCell = null;
            return false;
        }

        pointedCell = hit.transform.GetComponentInParent<Cell>();
        return true;
    }

    //private bool CheckCamera()
    //{
    //    if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
    //    {
    //        cameraMovement.position = cameraMovement.position == CameraMovement.positionType.White ? CameraMovement.positionType.Black : CameraMovement.positionType.White;
    //        return true;
    //    }
    //    if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
    //    {
    //        cameraMovement.position = cameraMovement.position != CameraMovement.positionType.Up ? CameraMovement.positionType.Up : CameraMovement.positionType.White;
    //        return true;
    //    }

    //    return false;
    //}

    private bool IsWin(Cell cell)
    {
        if (cell.lastPiece.type == PieceType.Wise) return false;
        if (cell.x == 0 && cell.pieces[0].team == 0 || cell.x == board.LineCount - 1 && cell.pieces[0].team == 1)
            return true;

        return false;
    }

    private void NextActionState()
    {
        if (config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            SM.ChangeState(State.Selection);
            return;
        }

        SM.ChangeState(State.PlayAuto);
    }

    private bool CheckReplayState()
    {
        if (replaySave == null) return false;

        // Next suivant
        if (replayAt != (-1, -1))
        {
            SM.ChangeState(State.Next);
            return true;
        }

        int turnId = save.turns.Count - 1;

        UI.replayButtons["Next"].interactable = true;

        if (replayState != ReplayState.Play && (canMove || canStack) && config.playerTypes[currentTeamId] == PlayerType.Human)
        {
            SM.ChangeState(State.Selection);
            return true;
        }

        // fin de tour
        if (save.turns[turnId].actions.Count == replaySave.turns[turnId].actions.Count)
        {
            SM.ChangeState(State.Turn);

            if (replaySave.turns[turnId + 1].actions.Count == 0)
                UI.replayButtons["Next"].interactable = false;

            if (replayState != ReplayState.Play && config.playerTypes[currentTeamId] == PlayerType.Human)
            {
                SM.ChangeState(State.PlayerTurn);
                return true;
            }
        }

        SM.ChangeState(State.Replay);
        return true;
    }
}
