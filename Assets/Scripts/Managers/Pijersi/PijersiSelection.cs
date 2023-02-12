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
        animation.HighlightDangers(null);
    }

    private void OnUpdateSelection()
    {
        // curseur hors plateau
        if (!CheckPointedCell())
        {
            if (mainAction.WasPressedThisFrame() || secondaryAction.WasPressedThisFrame())
            {
                if (canMove && canStack) // annule la selection
                {
                    SM.ChangeState(State.PlayerTurn);
                    return;
                }
            }

            Tooltip.Instance.Hide();
            return;
        }

        // curseur sur une case non-intéragible
        if (!validMoves.ContainsKey(pointedCell) || validMoves[pointedCell].Count == 0)
        {
            if (mainAction.WasPressedThisFrame() || secondaryAction.WasPressedThisFrame())
            {
                if (canMove && canStack) // annule la selection
                    SM.ChangeState(State.PlayerTurn);
                else if (pointedCell == selectedCell) // termine le tour
                {
                    UpdateUIAndReplay();
                    SM.ChangeState(State.Turn);
                }

                return;
            }

            // highlight
            if (pointedCell == selectedCell)
            {
                animation.HighlightDangers((selectedCellDangers[1] ?? selectedCellDangers[0])?.ToArray());

                if (pointedCell == lastPointedCell) return;

                if (canMove && canStack)
                {
                    Tooltip.Instance.Set("CancelSelection");
                    return;
                }
                Tooltip.Instance.Set("EndTurn");

                return;
            }

            animation.UpdateHighlight(pointedCell, ActionType.None);
            animation.HighlightDangers(null);
            if (pointedCell != lastPointedCell)
                Tooltip.Instance.Hide();

            return;
        }

        State[] orderedState;
        int actionId;
        ActionType[] alternateActions = { ActionType.Stack, ActionType.Unstack, ActionType.Move, ActionType.Attack };

        // action (ordre alternative)
        if (secondaryAction.WasPressedThisFrame())
        {
            UpdateUIAndReplay();
            Tooltip.Instance.Hide();

            orderedState = new State[] { State.Stack, State.Unstack, State.Move, State.Move };
            actionId = GetFirstValidActionId(alternateActions);

            if (actionId > -1)
            {
                SM.ChangeState(orderedState[actionId]);
                return;
            }

            if (canMove && canStack) // annule la selection
                SM.ChangeState(State.PlayerTurn);
            else if (pointedCell == selectedCell) // termine le tour
                SM.ChangeState(State.Turn);

            return;
        }

        // prend la première action valide
        ActionType[] actions = new ActionType[] { ActionType.Move, ActionType.Attack, ActionType.Stack, ActionType.Unstack };
        actionId = GetFirstValidActionId(actions);

        // action (défaut)
        if (mainAction.WasPressedThisFrame())
        {
            UpdateUIAndReplay();
            Tooltip.Instance.Hide();

            orderedState = new State[] { State.Move, State.Move, State.Stack, State.Unstack };

            if (actionId > -1)
                SM.ChangeState(orderedState[actionId]);
            else if (canMove && canStack)
                SM.ChangeState(State.PlayerTurn);
            else if (pointedCell == selectedCell)
                SM.ChangeState(State.Turn);

            return;
        }

        // highlights
        if (pointedCell != selectedCell)
            animation.UpdateHighlight(pointedCell, actionId == -1 ? ActionType.None : actions[actionId]);

        // actions tooltip
        if (pointedCell != lastPointedCell)
        {
            string tooltipKey = actionId > -1 ? actions[actionId].ToString() : "";

            actionId =GetFirstValidActionId(alternateActions);

            tooltipKey += actionId > -1 ? alternateActions[actionId].ToString() : "";

            //if (tooltipKey == "") // aucune action possible
            //{
            //    Tooltip.Instance.Hide();
            //    return;
            //}

            Tooltip.Instance.Set(tooltipKey);
        }

        if (this.dangers == null) return;

        List<Cell> dangers = this.dangers.ContainsKey(pointedCell) ? this.dangers[pointedCell] : null;
        if (!canMove)
        {
            dangers?.AddRange(selectedCellDangers[0]);
            dangers ??= selectedCellDangers[0];
            animation.HighlightDangers(dangers?.ToArray());
            return;
        }

        animation.HighlightDangers(dangers?.ToArray());

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
