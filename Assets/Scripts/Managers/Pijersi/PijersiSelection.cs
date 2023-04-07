using System.Linq;

public partial class Pijersi
{
    private void OnEnterSelection()
    {
        replayState = ReplayState.None;
        selectedCell = pointedCell;
        validMoves = selectedCell.lastPiece.GetLegalMoves(canMove, canStack);

        if (validMoves.Count == 0)
            SM.ChangeState(State.PlayerTurn);

        Cell[] cells = new Cell[validMoves.Keys.Count + 1];
        validMoves.Keys.CopyTo(cells, 0);
        cells[cells.Length - 1] = selectedCell;
        dangers[0] = selectedCell.pieces[0].GetDangers(board.pieces, cells);
        dangers[1] = selectedCell.pieces[1]?.GetDangers(board.pieces, cells);
        animation.NewSelection(selectedCell);
        animation.HighlightDangers(GetDangersFor(selectedCell, true));
        Tooltip.Instance.Set("CancelSelection");
    }

    private void OnExitSelection()
    {
        validMoves = null;
        selectedCell.ResetColor();
        pointedCell?.ResetColor();
        lastPointedCell?.ResetColor();
        animation.HighlightDangers(null);
        Tooltip.Instance.Hide();
    }

    private void OnUpdateSelection()
    {
        // cursor out of board
        if (!CheckPointedCell())
        {
            if (lastPointedCell == null) return;

            if (lastPointedCell != selectedCell)
                lastPointedCell.ResetColor();
            Tooltip.Instance.Hide();

            return;
        }

        // cursor on invalid target cell or on selectedCell
        if (!validMoves.ContainsKey(pointedCell) || validMoves[pointedCell].Count == 0)
        {
            // player cancel or end the turn
            if (mainAction.WasPressedThisFrame() || secondaryAction.WasPressedThisFrame())
            {
                EndSelection();
                return;
            }

            if (pointedCell == lastPointedCell) return;

            // highlight/tooltip
            if (pointedCell == selectedCell) // when on selectedCell
            {
                animation.HighlightDangers(GetDangersFor(selectedCell, true));
                lastPointedCell?.ResetColor();

                if (canMove && canStack)
                {
                    Tooltip.Instance.Set("CancelSelection");
                    return;
                }
                Tooltip.Instance.Set("EndTurn");

                return;
            }

            animation.UpdateHighlight(pointedCell);
            animation.HighlightDangers(null);
            Tooltip.Instance.Hide();

            return;
        }

        State[] orderedState;
        int actionId;
        ActionType[] alternateActions = { ActionType.Stack, ActionType.Unstack, ActionType.Move, ActionType.Attack };

        // action (alternative)
        if (secondaryAction.WasPressedThisFrame())
        {
            UpdateUIAndReplay();

            orderedState = new State[] { State.Stack, State.Unstack, State.Move, State.Move };
            actionId = GetFirstValidActionId(alternateActions);

            SM.ChangeState(orderedState[actionId]);

            return;
        }

        ActionType[] actions = new ActionType[] { ActionType.Move, ActionType.Attack, ActionType.Stack, ActionType.Unstack };
        actionId = GetFirstValidActionId(actions);

        // action (défaut)
        if (mainAction.WasPressedThisFrame())
        {
            UpdateUIAndReplay();

            orderedState = new State[] { State.Move, State.Move, State.Stack, State.Unstack };

            SM.ChangeState(orderedState[actionId]);

            return;
        }

        // actions highlights/tooltip
        if (pointedCell != lastPointedCell)
        {
            ActionType mainActionType = actions[actionId];

            // tooltip
            string tooltipKey = actions[actionId].ToString();
            actionId = GetFirstValidActionId(alternateActions);
            tooltipKey += alternateActions[actionId].ToString();

            Tooltip.Instance.Set(tooltipKey);

            // highlights
            if (pointedCell != selectedCell)
                animation.UpdateHighlight(pointedCell, mainActionType == alternateActions[actionId]);
        }

        if (dangers[0] == null && dangers[1] == null) return;

        // dangers highlight
        if (validMoves[pointedCell].Contains(ActionType.Unstack))
        {
            animation.HighlightDangers(GetDangersFor(pointedCell, true), GetDangersFor(selectedCell, false), GetDangersFor(pointedCell, false));
            return;
        }
        if (selectedCell.isFull && !selectedCell.nears.Contains(pointedCell))
        {
            animation.HighlightDangers(GetDangersFor(pointedCell, true), canStack ? GetDangersFor(pointedCell, false) : null);
            return;
        }
        animation.HighlightDangers(GetDangersFor(pointedCell, true));

        // cancel or end the selection
        void EndSelection()
        {
            // cancel selection
            if (canMove && canStack)
            {
                SM.ChangeState(State.PlayerTurn);
                return;
            }

            // end turn
            UpdateUIAndReplay();
            SM.ChangeState(State.Turn);
            return;
        }

        // return the first action in common between valid moves and ordered Actions
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
            UI.replayButtons["Back"].interactable = true;
            UI.replayButtons["Play"].interactable = false;
            UI.replayButtons["Next"].interactable = false;

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

        if (!dangers[0].ContainsKey(cell)) return null;

        return dangers[0][cell];
    }
}
