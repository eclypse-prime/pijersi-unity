using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private void UpdateCameraPosition()
    {
        if (teams[0].type != PlayerType.Human || teams[1].type != PlayerType.Human) 
            return;

        cameraMovement.position = (CameraMovement.PositionType) currentTeamId;
    }

    private void GetNextAiTurn(PlayerType nextTeamType)
    {
        int recursionDepth = (int)nextTeamType;
        playAuto = Task.Run(() =>
        {
            return engine.PlayAuto(recursionDepth);
        });
    }

    private bool CheckPointedCell()
    {
        lastPointedCell = pointedCell;

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 50f, cellLayer))
        {
            pointedCell = null;
            return false;
        }

        pointedCell = hit.transform.GetComponentInParent<Cell>();
        return true;
    }

    private bool IsWin(Cell cell)
    {
        if (cell.LastPiece.type == PieceType.Wise) return false;
        if (cell.x == 0 && cell.pieces[0].team == 0 || cell.x == board.LineCount - 1 && cell.pieces[0].team == 1)
            return true;

        return false;
    }

    private void NextActionState()
    {
        if (CurrentTeam.type == PlayerType.Human)
        {
            SM.ChangeState(State.Selection);
            return;
        }

        SM.ChangeState(State.PlayAuto);
    }

    private bool TryReplayState()
    {
        if (replaySave == null)
        {
            if (config.partyData == null || replayState != ReplayState.Play) return false;

            SM.ChangeState(State.AfterReplay);
            return true;
        }

        if (replayAt != (-1, -1))
        {
            SM.ChangeState(State.Next);
            return true;
        }

        int turnId = save.turns.Count - 1;

        UI.ReplayButtons["Next"].interactable = true;

        if (replayState != ReplayState.Play && (canMove || canStack) && CurrentTeam.type == PlayerType.Human)
        {
            SM.ChangeState(State.Selection);
            return true;
        }

        if (TryEndTurn()) return true;

        SM.ChangeState(State.Replay);
        return true;

        bool TryEndTurn()
        {
            if (save.turns[turnId].actions.Count != replaySave.turns[turnId].actions.Count)
                return false;

            SM.ChangeState(State.Turn);

            if (replaySave.turns[turnId + 1].actions.Count == 0)
                UI.ReplayButtons["Next"].interactable = false;

            if (replayState != ReplayState.Play && CurrentTeam.type == PlayerType.Human)
            {
                SM.ChangeState(State.PlayerTurn);
                return true;
            }
            return false;
        }
    }

    private void InitEngine()
    {
        if (teams[0].type == PlayerType.Human) return;

        engine = new Engine();
        GetNextAiTurn(teams[0].type);
    }
}
