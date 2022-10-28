using System;
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

        if (validMoves.Count == 0)
            SM.ChangeState(State.Turn);

        Cell[] cells = new Cell[validMoves.Keys.Count];
        validMoves.Keys.CopyTo(cells, 0);
        dangers = selectedCell.lastPiece.GetDangers(cells);
        selectedCellDangers[0] = selectedCell.pieces[0].GetDangers(selectedCell);
        selectedCellDangers[1] = selectedCell.pieces[1]?.GetDangers(selectedCell);
        animation.NewSelection(selectedCell);
        animation.HighlightDangers((selectedCellDangers[1] ?? selectedCellDangers[0])?.ToArray());
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
                {
                    CheckReplaySave();
                    UI.replayButtons["Next"].interactable = false;
                    SM.ChangeState(State.Turn);
                }

                return;
            }

            // highlight
            if (pointedCell == selectedCell)
            {
                animation.HighlightDangers((selectedCellDangers[1] ?? selectedCellDangers[0])?.ToArray());
                return;
            }

            animation.UpdateHighlight(pointedCell, ActionType.None);
            animation.HighlightDangers(null);
            return;
        }

        ActionType[] orderedActions;

        // action (ordre alternative)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CheckReplaySave();
            UI.replayButtons["Next"].interactable = false;

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
            CheckReplaySave();
            UI.replayButtons["Next"].interactable = false;

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

        List<Cell> dangers = this.dangers.ContainsKey(pointedCell) ? this.dangers[pointedCell] : null;
        if (!canMove)
        {
            dangers?.AddRange(selectedCellDangers[0]);
            dangers ??= selectedCellDangers[0];
            animation.HighlightDangers(dangers?.ToArray());
            return;
        }

        animation.HighlightDangers(dangers?.ToArray());
    }
    private void CheckReplaySave()
    {
        if (replaySave == null) return;

        replaySave = null;
        engine = null;
    }
}
