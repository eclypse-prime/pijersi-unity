using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private PijersiUI UI;
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;

    private State state;
    private bool isPauseOn;
    private new Camera camera;
    private PijersiEngine.Board engine;
    private Save save;
    private Cell pointedCell;
    private Cell selectedCell;
    private int currentTeam;
    private bool canMove;
    private bool canStack;
    private Dictionary<Cell, List<ActionType>> validMoves;
    private bool isUnstackAttack;
    private int[] playerScores;
    private string[] playerNames;

    private enum State
    {
        Turn,
        PlayerTurn,
        AiTurn,
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
        if (config.gameType != GameType.HumanVsHuman)
        {
            engine = new PijersiEngine.Board();
            engine.init();
        }
    }

    private void Start()
    {
        camera = Camera.main;
        state  = State.Turn;

        ResetMatch();
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
            case State.PlayerTurn:
                OnEnterPlayerTurn();
                break;
            case State.AiTurn:
                OnEnterAiTurn();
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
            case State.PlayerTurn:
                OnExitPlayerTurn();
                break;
            case State.AiTurn:
                OnExitAiTurn();
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
            case State.PlayerTurn:
                OnUpdatePlayerTurn();
                break;
            case State.AiTurn:
                OnUpdateAiTurn();
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
        save.AddTurn();
    }

    private void OnExitTurn()
    {
        UI.UpdateGameState(currentTeam, playerNames[currentTeam]);
        UI.AddRecordColumnLine(currentTeam);

    }

    private void OnUpdateTurn()
    {
        if (config.gameType == GameType.HumanVsHuman || currentTeam == config.playerId)
        {
            ChangeState(State.PlayerTurn);
            return;
        }

        ChangeState(State.AiTurn);
    }
    #endregion

    #region PlayerTurn
    private void OnEnterPlayerTurn() { }
    private void OnExitPlayerTurn() { }
    private void OnUpdatePlayerTurn()
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

    #region AiTurn
    private void OnEnterAiTurn() { }
    private void OnExitAiTurn() { }
    private void OnUpdateAiTurn() { }
    #endregion

    #region Selection
    private void OnEnterSelection()
    {
        selectedCell = pointedCell;
        validMoves = selectedCell.lastPiece.GetValidMoves(canMove, canStack);
        animation.NewSelection(selectedCell);

        if (validMoves.Count == 0)
            ChangeState(State.Turn);
    }

    private void OnExitSelection()
    {
        validMoves = null;
        selectedCell.ResetColor();
    }

    private void OnUpdateSelection()
    {
        if (!CheckPointedCell())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack)
                   ChangeState(State.PlayerTurn);
            }

            return;
        }

        if (!validMoves.ContainsKey(pointedCell) || validMoves[pointedCell].Count == 0)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (canMove && canStack)
                    ChangeState(State.PlayerTurn);
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
                if (validMoves[pointedCell].Contains(orderedActions[i]))
                {
                    ChangeState(orderedState[i]);
                    return;
                }
            }

            if (canMove && canStack)
                ChangeState(State.PlayerTurn);
            else if (pointedCell == selectedCell)
                ChangeState(State.Turn);

            return;
        }
        
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


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            State[] orderedState = { State.Move, State.Attack, State.Stack, State.Unstack };

            ChangeState(orderedState[actionId]);
            if (canMove && canStack)
                ChangeState(State.PlayerTurn);
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
        save.AddAction(ActionType.Move, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Move, canStack);
    }

    private void OnExitMove()
    {
    }

    private void OnUpdateMove()
    {
        if (board.UpdateMove(pointedCell)) return;

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
        save.AddAction(ActionType.Attack, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Attack, canStack);
    }

    private void OnExitAttack()
    {
    }

    private void OnUpdateAttack()
    {
        if (board.UpdateMove(pointedCell)) return;

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
        save.AddAction(ActionType.Stack, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Stack, canMove);
    }

    private void OnExitStack()
    {
    }

    private void OnUpdateStack()
    {
        if (board.UpdateMove(pointedCell)) return;

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
        ActionType action = !pointedCell.isEmpty ? ActionType.Attack : ActionType.Unstack;
        board.Unstack(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action, canMove);
        canMove = false;
    }

    private void OnExitUnstack()
    {
    }

    private void OnUpdateUnstack()
    {
        if (board.UpdateMove(pointedCell)) return;

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
        playerScores[currentTeam]++;
        UI.ShowEnd(currentTeam, playerScores, config.winRound);
        TogglePause();
    }
    private void OnExitEnd()
    {
        string firstName = playerNames[0];
        playerNames[0] = playerNames[1];
        playerNames[1] = firstName;

        int firstScore = playerScores[0];
        playerScores[0] = playerScores[1];
        playerScores[1] = firstScore;

        currentTeam = 1;
        board.ResetBoard();
        UI.ResetUI();
    }
    private void OnUpdateEnd()
    {
        if (playerScores[currentTeam] < config.winRound)
            ChangeState(State.Turn);
    }
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
    public void ResetMatch()
    {
        playerNames = new string[2];
        switch (config.gameType)
        {
            case GameType.HumanVsHuman:
                playerNames[0] = "Player #1";
                playerNames[1] = "Player #2";
                break;
            case GameType.HumanVsAi:
                playerNames[0] = "Player";
                playerNames[1] = "AI";
                break;
            case GameType.AiVsHuman:
                playerNames[0] = "AI";
                playerNames[1] = "Player";
                break;
            case GameType.AiVsAi:
                playerNames[0] = "AI #1";
                playerNames[1] = "AI #2";
                break;
            default:
                break;
        }

        playerScores = new int[2];
        currentTeam   = 1;
        board.ResetBoard();
        save = new Save(config.gameType);
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
        isPauseOn      = !isPauseOn;
        Time.timeScale = 1 - Time.timeScale;
        UI.SetActivePause(isPauseOn);
    }

    public void Save()
    {
        save.Write();
    }
    #endregion
}
