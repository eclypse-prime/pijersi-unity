using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
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

    private bool IsWin(Cell cell)
    {
        if (cell.lastPiece.type == PieceType.Wise) return false;
        if (cell.x == 0 && cell.pieces[0].team == 0 || cell.x == board.LineCount - 1 && cell.pieces[0].team == 1)
            return true;

        return false;
    }

    private void NextActionState()
    {
        if (CurrentTeam.Type == PlayerType.Human)
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

        if (replayState != ReplayState.Play && (canMove || canStack) && CurrentTeam.Type == PlayerType.Human)
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

            if (replayState != ReplayState.Play && CurrentTeam.Type == PlayerType.Human)
            {
                SM.ChangeState(State.PlayerTurn);
                return true;
            }
        }

        SM.ChangeState(State.Replay);
        return true;
    }

    private void InitEngine()
    {
        if (teams[0].Type != PlayerType.Human)
        {
            engine = new Engine();
            GetNextAiTurn(teams[0].Type);
        }
        else if (teams[1].Type != PlayerType.Human)
            engine = new Engine();
    }
}
