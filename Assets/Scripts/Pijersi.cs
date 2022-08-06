using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pijersi : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;
    
    private enum State
    {
        Wait,
        Ready,
        Selection,
        Move
    }

    private State state;
    private new Camera camera;
    private Cell pointedCell;
    private Cell selectedCell;
    private List<Cell> valideMoves;
    private int currentTeam = 0;

    public static Pijersi Instance;

    #region base
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state   = State.Ready;
        camera  = Camera.main;
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
            case State.Ready:
                OnEnterReady();
                break;
            case State.Selection:
                OnEnterSelection();
                break;
            case State.Move:
                OnEnterMove();
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
            case State.Ready:
                OnExitReady();
                break;
            case State.Selection:
                OnExitSelection();
                break;
            case State.Move:
                OnExitMove();
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
            case State.Ready:
                OnUpdateReady();
                break;
            case State.Selection:
                OnUpdateSelection();
                break;
            case State.Move:
                OnUpdateMove();
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

    #region Ready
    private void OnEnterReady() { }
    private void OnExitReady() { }
    private void OnUpdateReady()
    {
        if (!CheckPointedCell()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && !pointedCell.isEmpty && pointedCell.pieces[0].team == currentTeam)
        {
            ChangeState(State.Selection);
            return;
        }

        animation.UpdateHighlight(pointedCell.transform);
    }
    #endregion

    #region Selection
    private void OnEnterSelection()
    {
        selectedCell = pointedCell;
        valideMoves = board.GetValideMoves(selectedCell);
        animation.NewSelection(selectedCell);
    }

    private void OnExitSelection()
    {
        selectedCell.ResetColor();
    }

    private void OnUpdateSelection()
    {
        if (!CheckPointedCell()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && valideMoves.Contains(pointedCell))
        {
            ChangeState(State.Move);
            return;
        }

        if (pointedCell == selectedCell) return;
        animation.UpdateHighlight(pointedCell.transform);
    }
    #endregion

    #region Move
    private void OnEnterMove()
    {
        board.MovePiece(selectedCell, pointedCell);
    }
    private void OnExitMove()
    {
        selectedCell = null;
    }
    private void OnUpdateMove()
    {
        ChangeState(State.Ready);
    }
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

        pointedCell = hit.transform.GetComponent<Cell>();
        return true;
    }

    private bool IsValideMove(Cell move)
    {
        foreach (Cell valideMove in valideMoves)
        {
            if (move == valideMove)
                return true;
        }

        return false;
    }
    #endregion
}
