using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pijersi : MonoBehaviour
{
    [SerializeField] private PijersiUI UI;
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;

    private State state;
    private bool isPauseOn;
    private new Camera camera;
    private Cell pointedCell;
    private Cell selectedCell;
    private int currentTeam;
    private bool canMove;
    private bool canStack;
    private Dictionary<Cell, List<ActionType>> valideMoves;
    private bool isUnstackAttack;

    public static Pijersi Instance;

    private enum State
    {
        Turn,
        Ready,
        Selection,
        Move,
        Attack,
        Stack,
        Unstack,
        End
    }

    #region base
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        camera      = Camera.main;
        state       = State.Turn;
        currentTeam = 1;
        OnStateEnter();
    }

    private void Update()
    {
        if (CheckPause()) return;

        OnStateUpdate();
    }
    #endregion

    #region State Machine
    private void OnStateEnter()
    {
        switch (state)
        {
            case State.Turn:
                OnEnterTurn();
                break;
            case State.Ready:
                OnEnterReady();
                break;
            case State.Selection:
                OnEnterSelection();
                break;
            case State.Move:
                OnEnterMove();
                break;
            case State.Attack:
                OnEnterAttack();
                break;
            case State.Stack:
                OnEnterStack();
                break;
            case State.Unstack:
                OnEnterUnstack();
                break;
            case State.End:
                OnEnterEnd();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }

    private void OnStateExit()
    {
        switch (state)
        {
            case State.Turn:
                OnExitTurn();
                break;
            case State.Ready:
                OnExitReady();
                break;
            case State.Selection:
                OnExitSelection();
                break;
            case State.Move:
                OnExitMove();
                break;
            case State.Attack:
                OnExitAttack();
                break;
            case State.Stack:
                OnExitStack();
                break;
            case State.Unstack:
                OnExitUnstack();
                break;
            case State.End:
                OnExitEnd();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }

    private void OnStateUpdate()
    {
        switch (state)
        {
            case State.Turn:
                OnUpdateTurn();
                break;
            case State.Ready:
                OnUpdateReady();
                break;
            case State.Selection:
                OnUpdateSelection();
                break;
            case State.Move:
                OnUpdateMove();
                break;
            case State.Attack:
                OnUpdateAttack();
                break;
            case State.Stack:
                OnUpdateStack();
                break;
            case State.Unstack:
                OnUpdateUnstack();
                break;
            case State.End:
                OnUpdateEnd();
                break;
            default:
                Debug.Log($"état non implémenté: {state}");
                break;
        }
    }

    private void ChangeState(State newState)
    {
        OnStateExit();
        state = newState;
        OnStateEnter();
    }
    #endregion

    #region Turn
    private void OnEnterTurn()
    {
        currentTeam = 1 - currentTeam;
        canMove     = true;
        canStack    = true;
    }
    private void OnExitTurn() { }
    private void OnUpdateTurn()
    {
        ChangeState(State.Ready);
    }
    #endregion

    #region Ready
    private void OnEnterReady() { }
    private void OnExitReady() { }
    private void OnUpdateReady()
    {
        if (!CheckPointedCell()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && pointedCell.pieces[0]?.team == currentTeam)
        {
            ChangeState(State.Selection);
            return;
        }

        animation.UpdateHighlight(pointedCell);
    }
    #endregion

    #region Selection
    private void OnEnterSelection()
    {
        selectedCell = pointedCell;
        valideMoves = selectedCell.lastPiece.GetValidMoves(canMove, canStack);
        animation.NewSelection(selectedCell);

        if (valideMoves.Count == 0)
            ChangeState(State.Turn);
    }

    private void OnExitSelection()
    {
        valideMoves = null;
        selectedCell.ResetColor();
    }

    private void OnUpdateSelection()
    {
        if (!CheckPointedCell())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack)
                   ChangeState(State.Ready);
            }

            return;
        }

        if (!valideMoves.ContainsKey(pointedCell) || valideMoves[pointedCell].Count == 0)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack)
                    ChangeState(State.Ready);
                else if (pointedCell == selectedCell)
                    ChangeState(State.Turn);

                return;
            }

            if (pointedCell != selectedCell)
                animation.UpdateHighlight(pointedCell, ActionType.None);
            return;
        }

        ActionType[] orderedActions;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            orderedActions = new ActionType[] { ActionType.Stack, ActionType.Unstack, ActionType.Move, ActionType.Attack };
            State[] orderedState = { State.Stack, State.Unstack, State.Move, State.Attack };

            for (int i = 0; i < orderedActions.Length; i++)
            {
                if (valideMoves[pointedCell].Contains(orderedActions[i]))
                {
                    ChangeState(orderedState[i]);
                    return;
                }
            }

            if (canMove && canStack)
                ChangeState(State.Ready);
            else if (pointedCell == selectedCell)
                ChangeState(State.Turn);

            return;
        }
        
        orderedActions = new ActionType[] { ActionType.Move, ActionType.Attack, ActionType.Stack, ActionType.Unstack };
        int actionId = -1;
        for (int i = 0; i < orderedActions.Length; i++)
        {
            if (valideMoves[pointedCell].Contains(orderedActions[i]))
            {
                actionId = i;
                break;
            }
        }


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            State[] orderedState = { State.Move, State.Attack, State.Stack, State.Unstack };

            ChangeState(orderedState[actionId]);
            if (canMove && canStack)
                ChangeState(State.Ready);
            else if (pointedCell == selectedCell)
                ChangeState(State.Turn);

            return;
        }

        // highlights
        if (pointedCell != selectedCell)
            animation.UpdateHighlight(pointedCell, actionId == -1 ? ActionType.None : orderedActions[actionId]);
    }
    #endregion

    #region Move
    private void OnEnterMove()
    {
        canMove = false;
        board.Move(selectedCell, pointedCell);
    }

    private void OnExitMove()
    {
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Move, canStack);
    }

    private void OnUpdateMove()
    {
        if (IsWin(pointedCell))
        {
            ChangeState(State.End);
            return;
        }

        if (canStack)
        {
            ChangeState(State.Selection);
            return;
        }

        ChangeState(State.Turn);
    }
    #endregion

    #region Attack
    private void OnEnterAttack()
    {
        canMove = false;
        board.Attack(selectedCell, pointedCell);
    }

    private void OnExitAttack()
    {
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Attack, canStack);
    }

    private void OnUpdateAttack()
    {
        if (IsWin(pointedCell))
        {
            ChangeState(State.End);
            return;
        }

        if (canStack)
        {
            ChangeState(State.Selection);
            return;
        }

        ChangeState(State.Turn);
    }
    #endregion

    #region Stack
    private void OnEnterStack()
    {
        canStack = false;
        board.Stack(selectedCell, pointedCell);
    }
    private void OnExitStack()
    {
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Stack, canMove);
    }
    private void OnUpdateStack()
    {
        if (IsWin(pointedCell))
        {
            ChangeState(State.End);
            return;
        }

        if (canMove)
        {
            ChangeState(State.Selection);
            return;
        }

        ChangeState(State.Turn);
    }
    #endregion

    #region Unstack
    private void OnEnterUnstack()
    {
        canStack = false;
        canMove = false;
        isUnstackAttack = !pointedCell.isEmpty;
        board.Unstack(selectedCell, pointedCell);
    }
    private void OnExitUnstack()
    {
        UI.UpdateRecord(selectedCell, pointedCell, isUnstackAttack ? ActionType.Attack : ActionType.Unstack, canMove);
    }
    private void OnUpdateUnstack()
    {
        if (IsWin(pointedCell))
        {
            ChangeState(State.End);
            return;
        }

        ChangeState(State.Turn);
    }
    #endregion

    #region End
    private void OnEnterEnd()
    {
        UI.UpdateRecord(currentTeam);
    }
    private void OnExitEnd()
    {
        ResetMatch(true);
    }
    private void OnUpdateEnd() { }
    #endregion

    /*
        #region 
        private void OnEnter() { }
        private void OnExit() { }
        private void OnUpdate() { }
        #endregion
    */

    #region check
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

    private bool CheckPause()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        return isPauseOn;
    }
    #endregion

    #region common
    public void ResetMatch(bool isMatchEnd = false)
    {
        currentTeam = 1;
        if (!isMatchEnd)
        {
            ChangeState(State.Turn);
            TogglePause();
        }

        board.ResetBoard();
        UI.ResetUI();
    }

    private bool IsWin(Cell cell)
    {
        if (cell.x == board.LineCount - 1 && cell.pieces[0].team == 0 || cell.x == 0 && cell.pieces[0].team == 1)
            return true;

        return false;
    }

    public void TogglePause()
    {
        isPauseOn = !isPauseOn;
        Time.timeScale = 1 - Time.timeScale;
        UI.SetActivePause(isPauseOn);
    }
    #endregion
}
