using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pijersi : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;

    private State state;
    private new Camera camera;
    private bool isPauseOn = false;
    private Cell pointedCell;
    private Cell selectedCell;
    private int currentTeam = 1;
    private bool canMove;
    private bool canStack;
    private Dictionary<ActionType, List<Cell>> valideMoves;

    public static Pijersi Instance;
    private enum State
    {
        Turn,
        Ready,
        Selection,
        Move,
        Attack,
        Stack,
        End
    }

    #region base
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        camera  = Camera.main;
        state   = State.Turn;
        OnStateEnter();
    }

    private void Update()
    {
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
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isPauseOn = !isPauseOn;
            return;
        }
        if (isPauseOn) return;

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
        valideMoves  = board.GetValideMoves(selectedCell, canMove, canStack);
        animation.NewSelection(selectedCell);
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
            if (canMove && canStack && Mouse.current.leftButton.wasPressedThisFrame)
                ChangeState(State.Ready);
            return;
        }

        ActionType action = FindCellAction();

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            switch (action)
            {
                case ActionType.none:
                    if (canMove && canStack)
                        ChangeState(State.Ready);
                    else if (pointedCell == selectedCell)
                        ChangeState(State.Turn);
                    break;
                case ActionType.stack:
                case ActionType.unstack:
                    ChangeState(State.Stack);
                    break;
                case ActionType.move:
                    ChangeState(State.Move);
                    break;
                case ActionType.attack:
                    ChangeState(State.Attack);
                    break;
                default:
                    break;
            }

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            switch (action)
            {
                case ActionType.none:
                    if (canMove && canStack)
                        ChangeState(State.Ready);
                    else if (pointedCell == selectedCell)
                        ChangeState(State.Turn);
                    break;
                case ActionType.move:
                    ChangeState(State.Move);
                    break;
                case ActionType.attack:
                    ChangeState(State.Attack);
                    break;
                case ActionType.stack:
                case ActionType.unstack:
                    ChangeState(State.Stack);
                    break;
                default:
                    break;
            }

            return;
        }

        // highlights
        if (pointedCell != selectedCell)
            animation.UpdateHighlight(pointedCell, action);
    }
    #endregion

    #region Move
    private void OnEnterMove()
    {
        canMove = false;
        board.Move(selectedCell, pointedCell);
    }
    private void OnExitMove() { }
    private void OnUpdateMove()
    {
        if (board.IsWin(pointedCell))
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
    private void OnExitAttack() { }
    private void OnUpdateAttack()
    {
        if (board.IsWin(pointedCell))
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
        board.StackUnstask(selectedCell, pointedCell);
    }
    private void OnExitStack() { }
    private void OnUpdateStack()
    {
        if (board.IsWin(pointedCell))
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

    #region End
    private void OnEnterEnd()
    {
        Debug.Log("End");
    }
    private void OnExitEnd()
    {
        currentTeam = 1;
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

    #region common
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

    private ActionType FindCellAction()
    {
        foreach (KeyValuePair<ActionType, List<Cell>> actionCells in valideMoves)
        {
            if (actionCells.Value.Contains(pointedCell))
                return actionCells.Key;
        }

        return ActionType.none;
    }
    #endregion
}
