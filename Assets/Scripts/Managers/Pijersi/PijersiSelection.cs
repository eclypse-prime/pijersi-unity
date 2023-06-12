using System.Linq;

public partial class Pijersi
{
    private void OnEnterSelection()
    {
        replayState = ReplayState.None;
        selectedCell = pointedCell;
        validMoves = selectedCell.LastPiece.GetLegalMoves(canMove, canStack);

        if (validMoves.Count == 0)
            SM.ChangeState(State.PlayerTurn);

        Cell[] cells = new Cell[validMoves.Keys.Count + 1];
        validMoves.Keys.CopyTo(cells, 0);
        cells[cells.Length - 1] = selectedCell;
        dangers[0] = selectedCell.pieces[0].GetDangers(board.Pieces, cells);
        dangers[1] = selectedCell.pieces[1]?.GetDangers(board.Pieces, cells);
        animation.NewSelection(selectedCell);
        animation.HighlightDangers(GetDangersFor(selectedCell, true));
        Tooltip.Set("CancelSelection");
    }

    private void OnExitSelection()
    {
        validMoves = null;
        selectedCell.ResetColor();
        pointedCell?.ResetColor();
        lastPointedCell?.ResetColor();
        animation.HighlightDangers(null);
        Tooltip.Hide();
    }

    private void OnUpdateSelection()
    {
        if (!CheckPointedCell())
        {
            CheckOutOfBoard();
            return;
        }

        if (TryOnInvalidTargetOrSelectedCell()) return;

        State[] orderedState;
        int actionId;
        ActionType[] alternateActions = { ActionType.Stack, ActionType.Unstack, ActionType.Move, ActionType.Attack };

        if (TryAlternativeAction()) return;

        ActionType[] actions = new ActionType[] { ActionType.Move, ActionType.Attack, ActionType.Stack, ActionType.Unstack };
        actionId = GetFirstValidActionId(actions);

        if (TryDefaultAction()) return;
        TooltipAndActionHighlight();
        HighlightDangers();

        void CheckOutOfBoard()
        {
            if (lastPointedCell == null) return;

            if (lastPointedCell != selectedCell)
                lastPointedCell.ResetColor();
            Tooltip.Hide();
        }

        bool TryOnInvalidTargetOrSelectedCell()
        {
            if (validMoves.ContainsKey(pointedCell) && validMoves[pointedCell].Count != 0) 
                return false;

            if (TryPlayerCancelOrEndTurn()) return true;

            if (pointedCell == lastPointedCell) return true;

            HighlightTooltip();

            return true;
        }

        bool TryPlayerCancelOrEndTurn()
        {
            if (!mainAction.WasPressedThisFrame() && !secondaryAction.WasPressedThisFrame())
                return false;
            
            if (canMove && canStack)
            {
                SM.ChangeState(State.PlayerTurn);
                return true;
            }

            UpdateUIAndReplay();
            UpdateEngine();
            SM.ChangeState(State.Turn);
            return true;
        }

        void HighlightTooltip()
        {
            if (pointedCell == selectedCell)
            {
                animation.HighlightDangers(GetDangersFor(selectedCell, true));
                lastPointedCell?.ResetColor();

                if (canMove && canStack)
                {
                    Tooltip.Set("CancelSelection");
                    return;
                }
                Tooltip.Set("EndTurn");

                return;
            }
            animation.UpdateHighlight(pointedCell);
            animation.HighlightDangers(null);
            Tooltip.Hide();
        }

        bool TryAlternativeAction()
        {
            if (!secondaryAction.WasPressedThisFrame()) return false;

            UpdateUIAndReplay();

            orderedState = new State[] { State.Stack, State.Unstack, State.Move, State.Move };
            actionId = GetFirstValidActionId(alternateActions);
            SM.ChangeState(orderedState[actionId]);
            if (!canMove && !canStack)
                UpdateEngine();

            return true;
        }

        bool TryDefaultAction()
        {
            if (!mainAction.WasPressedThisFrame()) return false;
            
            UpdateUIAndReplay();

            orderedState = new State[] { State.Move, State.Move, State.Stack, State.Unstack };
            SM.ChangeState(orderedState[actionId]);
            if (!canMove && !canStack)
                UpdateEngine();

            return true;
        }

        void TooltipAndActionHighlight()
        {
            if (pointedCell == lastPointedCell) return;

            ActionType mainActionType = actions[actionId];

            string tooltipKey = actions[actionId].ToString();
            actionId = GetFirstValidActionId(alternateActions);
            tooltipKey += alternateActions[actionId].ToString();
            Tooltip.Set(tooltipKey);

            if (pointedCell != selectedCell)
                animation.UpdateHighlight(pointedCell, mainActionType == alternateActions[actionId]);
        }

        void HighlightDangers()
        {
            if (dangers[0] == null && dangers[1] == null) return;

            if (validMoves[pointedCell].Contains(ActionType.Unstack))
            {
                animation.HighlightDangers(GetDangersFor(pointedCell, true), GetDangersFor(selectedCell, false), GetDangersFor(pointedCell, false));
                return;
            }
            if (selectedCell.IsFull && !selectedCell.Nears.Contains(pointedCell))
            {
                animation.HighlightDangers(GetDangersFor(pointedCell, true), canStack ? GetDangersFor(pointedCell, false) : null);
                return;
            }
            animation.HighlightDangers(GetDangersFor(pointedCell, true));
        }

        int GetFirstValidActionId(ActionType[] orderedActions)
        {
            int actionId = -1;
            for (int i = 0; i < orderedActions.Length; i++)
            {
                if (validMoves[pointedCell].Contains(orderedActions[i]))
                {
                    actionId = i;
                    break;
                }
            }

            return actionId;
        }

        void UpdateUIAndReplay()
        {
            UI.ReplayButtons["Back"].interactable = true;
            UI.ReplayButtons["Play"].interactable = false;
            UI.ReplayButtons["Next"].interactable = false;

            if (replaySave == null) return;

            replaySave = null;
            engine = null;
        }
    }

    private Cell[] GetDangersFor(Cell cell, bool isTopPiece)
    {
        if (isTopPiece)
        {
            if ((dangers[1] ?? dangers[0])?.ContainsKey(cell) != true) return null;
            
            return (dangers[1] ?? dangers[0])[cell];
        }

        if (dangers[0]?.ContainsKey(cell) != true) return null;

        return dangers[0][cell];
    }

    private void UpdateEngine()
    {
        if (OtherTeam.type == PlayerType.Human) return;

        if (engine == null)
        {
            engine = new Engine();
            engine.SetState(board.GetState());
            engine.SetPlayer((byte)currentTeamId);
        }

        int[] manualPlay = new int[3];
        Save.Turn lastTurn = save.turns[^1];
        manualPlay[0] = board.CoordsToIndex(lastTurn.cells[0].x, lastTurn.cells[0].y);

        if (TryGetSimpleAction()) return;

        manualPlay[1] = board.CoordsToIndex(lastTurn.cells[1].x, lastTurn.cells[1].y);
        manualPlay[2] = board.CoordsToIndex(lastTurn.cells[2].x, lastTurn.cells[2].y);

        engine.PlayManual(manualPlay);
        GetNextAiTurn(OtherTeam.type);

        bool TryGetSimpleAction()
        {
            if (lastTurn.actions.Count >= 2) return false;

            bool isUnstack = lastTurn.actions[0] == ActionType.Unstack || lastTurn.actions[0] == ActionType.Stack;

            manualPlay[1] = isUnstack ? board.CoordsToIndex(lastTurn.cells[0].x, lastTurn.cells[0].y) : 255;
            manualPlay[2] = board.CoordsToIndex(lastTurn.cells[1].x, lastTurn.cells[1].y);

            engine.PlayManual(manualPlay);
            GetNextAiTurn(OtherTeam.type);
            return true;
        }
    }
}
