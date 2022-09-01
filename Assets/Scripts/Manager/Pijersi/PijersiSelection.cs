using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi
{
    private void OnEnterSelection()
    {
        selectedCell = pointedCell;
        validMoves = selectedCell.lastPiece.GetValidMoves(canMove, canStack);
        Cell[] cells = new Cell[validMoves.Keys.Count];
        validMoves.Keys.CopyTo(cells, 0);
        dangers = selectedCell.lastPiece.GetDanger(cells);
        animation.NewSelection(selectedCell);

        if (validMoves.Count == 0)
            SM.ChangeState(State.Turn);
    }

    private void OnExitSelection()
    {
        validMoves = null;
        selectedCell.ResetColor();
        UI.replayButtons["Back"].interactable = true;
    }

    private void OnUpdateSelection()
    {
        // curseur hors plateau
        if (!CheckPointedCell())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack) // annule la selection
                    SM.ChangeState(State.PlayerTurn);
            }

            return;
        }

        // curseur sur une case non-intéragible
        if (!validMoves.ContainsKey(pointedCell) || validMoves[pointedCell].Count == 0)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack) // annule la selection
                    SM.ChangeState(State.PlayerTurn);
                else if (pointedCell == selectedCell) // termine le tour
                    SM.ChangeState(State.Turn);

                return;
            }

            if (pointedCell != selectedCell) // highlight
            {
                animation.UpdateHighlight(pointedCell, ActionType.None);
                animation.HighlightDangers(null);
            }
            return;
        }

        ActionType[] orderedActions;

        // action (ordre alternative)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            orderedActions = new ActionType[] { ActionType.Stack, ActionType.Unstack, ActionType.Move, ActionType.Attack };
            State[] orderedState = { State.Stack, State.Unstack, State.Move, State.Move };

            for (int i = 0; i < orderedActions.Length; i++)
            {
                // lance la première action valide
                if (validMoves[pointedCell].Contains(orderedActions[i]))
                {
                    SM.ChangeState(orderedState[i]);
                    return;
                }
            }

            if (canMove && canStack) // annule la selection
                SM.ChangeState(State.PlayerTurn);
            else if (pointedCell == selectedCell) // termine le tour
                SM.ChangeState(State.Turn);

            return;
        }

        // prend la première action valide
        orderedActions = new ActionType[] { ActionType.Move, ActionType.Attack, ActionType.Stack, ActionType.Unstack };
        int actionId = -1;
        for (int i = 0; i < orderedActions.Length; i++)
        {
            if (validMoves[pointedCell].Contains(orderedActions[i]))
            {
                actionId = i;
                break;
            }
        }

        // action (défaut)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            State[] orderedState = { State.Move, State.Move, State.Stack, State.Unstack };

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
            animation.UpdateHighlight(pointedCell, actionId == -1 ? ActionType.None : orderedActions[actionId]);

        if (this.dangers == null) return;

        Cell[] dangers = this.dangers.ContainsKey(pointedCell) ? this.dangers[pointedCell].ToArray() : null;
        animation.HighlightDangers(dangers);
    }
}
