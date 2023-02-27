using System.Collections.Generic;

public partial class Pijersi
{
    private void OnEnterSelection()
    {
        replayState = ReplayState.None;
        selectedCell = pointedCell;
        validMoves = selectedCell.lastPiece.GetValidMoves(canMove, canStack);

        if (validMoves.Count == 0)
            SM.ChangeState(State.PlayerTurn);

        Cell[] cells = new Cell[validMoves.Keys.Count];
        validMoves.Keys.CopyTo(cells, 0);
        dangers = selectedCell.lastPiece.GetDangers(cells);
        selectedCellDangers[0] = selectedCell.pieces[0].GetDangers(selectedCell);
        selectedCellDangers[1] = selectedCell.pieces[1]?.GetDangers(selectedCell);
        animation.NewSelection(selectedCell);
        animation.HighlightDangers((selectedCellDangers[1] ?? selectedCellDangers[0])?.ToArray());
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

            // player cancel or end the turn
            if (mainAction.WasReleasedThisFrame() || secondaryAction.WasPressedThisFrame())
                EndSelection();

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
            }

            // highlight/tooltip
            if (pointedCell == selectedCell) // when on selectedCell
            {
                animation.HighlightDangers((selectedCellDangers[1] ?? selectedCellDangers[0])?.ToArray());

                if (pointedCell == lastPointedCell) return;

                lastPointedCell?.ResetColor();

                if (canMove && canStack)
                {
                    Tooltip.Instance.Set("CancelSelection");
                    return;
                }
                Tooltip.Instance.Set("EndTurn");

                return;
            }

            if (pointedCell == lastPointedCell) return;

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
            // highlights
            if (pointedCell != selectedCell)
                animation.UpdateHighlight(pointedCell, actions[actionId]);

            // tooltip
            string tooltipKey = actions[actionId].ToString();
            actionId = GetFirstValidActionId(alternateActions);
            tooltipKey += alternateActions[actionId].ToString();

            Tooltip.Instance.Set(tooltipKey);
        }

        if (this.dangers == null) return;

        // dangers highlight
        List<Cell> dangers = this.dangers.ContainsKey(pointedCell) ? this.dangers[pointedCell] : null;
        if (!canMove) // add bottom piece dangers if can only unstack
        {
            dangers?.AddRange(selectedCellDangers[0]);
            dangers ??= selectedCellDangers[0];
            animation.HighlightDangers(dangers?.ToArray());
            return;
        }

        animation.HighlightDangers(dangers?.ToArray());


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
}
