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
    private IEngine engine;
    private Save save;
    private Cell pointedCell;
    private Cell selectedCell;
    private int currentTeamId;
    private bool canMove;
    private bool canStack;
    private Dictionary<Cell, List<ActionType>> validMoves;
    private int[] playerScores;
    private string[] playerNames;
    private int[] playAuto;
    private State[] aiActionStates;
    private Cell[] aiActionCells;

    private enum State
    {
        Turn,
        PlayerTurn,
        AiTurn,
        Selection,
        PlayAuto,
        Move,
        Attack,
        Stack,
        Unstack,
        End,
    }

    #region base
    private void Start()
    {
        camera = Camera.main;
        state  = State.Turn;

        ResetMatch(true);
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
            case State.PlayAuto:
                OnEnterPlayAuto();
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
            case State.PlayAuto:
                OnExitPlayAuto();
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
            case State.PlayAuto:
                OnUpdatePlayAuto();
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
        AddManualPlay();

        currentTeamId = 1 - currentTeamId;
        canMove       = true;
        canStack      = true;
        selectedCell  = null;
        pointedCell   = null;
        save.AddTurn();
    }

    private void OnExitTurn()
    {
        UI.UpdateGameState(currentTeamId, playerNames[currentTeamId]);
        UI.AddRecordColumnLine(currentTeamId);
    }

    private void OnUpdateTurn()
    {
        if (config.playerTypes[currentTeamId] == PlayerType.Human)
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

        if (Mouse.current.leftButton.wasPressedThisFrame && pointedCell.pieces[0]?.team == currentTeamId)
        {
            ChangeState(State.Selection);
            return;
        }

        animation.UpdateHighlight(pointedCell);
    }
    #endregion

    #region AiTurn
    private void OnEnterAiTurn()
    {
        aiActionStates = new State[2];
        aiActionCells = new Cell[3];
        playAuto = null;
        StartCoroutine(PlayAuto());
    }

    private void OnExitAiTurn() { }

    private void OnUpdateAiTurn()
    {
        if (playAuto == null) return;

        aiActionCells[0] = board.cells[board.CoordsToIndex(playAuto[0], playAuto[1])];
        if (playAuto[2] > -1)
            aiActionCells[1] = board.cells[board.CoordsToIndex(playAuto[2], playAuto[3])];
        aiActionCells[2] = board.cells[board.CoordsToIndex(playAuto[4], playAuto[5])];

        // actions simples
        if (aiActionCells[1] == null) // move
        {
            canStack       = false;
            aiActionStates = new State[] { State.Move };
            aiActionCells  = new Cell[] { aiActionCells[0], aiActionCells[2] };
            ChangeState(State.PlayAuto);
            return;
        }

        if (aiActionCells[1] == aiActionCells[0]) // (un)stack
        {
            canMove         = false;
            State newState  = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team ? State.Stack : State.Unstack;
            aiActionStates  = new State[] { newState };
            aiActionCells   = new Cell[] { aiActionCells[0], aiActionCells[2] };
            ChangeState(State.PlayAuto);
            return;
        }

        // actions composées
        if (aiActionCells[1].isEmpty) // move -> (un)stack
        {
            aiActionStates[1] = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team ? State.Stack : State.Unstack;
            aiActionStates[0] = State.Move;
        }
        else if (aiActionCells[1].pieces[0].team != aiActionCells[0].pieces[0].team) // attack -> (un)stack
        {
            aiActionStates[1] = aiActionCells[2].pieces[0]?.team == aiActionCells[0].pieces[0].team ? State.Stack : State.Unstack;
            aiActionStates[0] = State.Attack;
        }
        else // stack -> move/attack
        {
            aiActionStates[1] = aiActionCells[2].isEmpty ? State.Move : State.Attack;
            aiActionStates[0] = State.Stack;
        }
        ChangeState(State.PlayAuto);
    }
    #endregion

    #region Selection
    private void OnEnterSelection()
    {
        selectedCell = pointedCell;
        validMoves   = selectedCell.lastPiece.GetValidMoves(canMove, canStack);
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

    #region PlayAuto
    private void OnEnterPlayAuto() { }
    private void OnExitPlayAuto() { }

    private void OnUpdatePlayAuto()
    {
        selectedCell = aiActionCells[0];
        pointedCell  = aiActionCells[1];
        ChangeState(aiActionStates[0]);

        if (aiActionStates.Length < 2) return;

        aiActionCells  = new Cell[] { aiActionCells[1], aiActionCells[2] };
        aiActionStates = new State[] { aiActionStates[1] };
    }
    #endregion

    #region Move
    private void OnEnterMove()
    {
        canMove = false;
        ActionType action = pointedCell.isEmpty ? ActionType.Move : ActionType.Attack;
        board.Move(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action);
    }

    private void OnExitMove() { }

    private void OnUpdateMove()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (IsWin(pointedCell))
        {
            ChangeState(State.End);
            return;
        }

        if (canStack && pointedCell.isFull)
        {
            if (config.playerTypes[currentTeamId] != PlayerType.Human)
            {
                ChangeState(State.PlayAuto);
                return;
            }

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
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Attack);
    }

    private void OnExitAttack() { }

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
            if (config.playerTypes[currentTeamId] != PlayerType.Human)
            {
                ChangeState(State.PlayAuto);
                return;
            }

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
        UI.UpdateRecord(selectedCell, pointedCell, ActionType.Stack);
    }

    private void OnExitStack() { }

    private void OnUpdateStack()
    {
        if (board.UpdateMove(pointedCell)) return;

        if (canMove)
        {
            if (config.playerTypes[currentTeamId] != PlayerType.Human)
            {
                ChangeState(State.PlayAuto);
                return;
            }

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
        ActionType action = pointedCell.isEmpty ? ActionType.Unstack : ActionType.Attack;
        board.Unstack(selectedCell, pointedCell);
        save.AddAction(action, selectedCell, pointedCell);
        UI.UpdateRecord(selectedCell, pointedCell, action);
        canMove = false;
    }

    private void OnExitUnstack() { }

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
        playerScores[currentTeamId]++;
        UI.ShowEnd(currentTeamId, playerScores, config.winMax);
        TogglePause();
    }

    private void OnExitEnd()
    {
        if (config.playerTypes[0] != PlayerType.Human || config.playerTypes[1] != PlayerType.Human)
            engine = new Engine();
        save = new Save(config.playerTypes);

        string firstName = playerNames[0];
        playerNames[0]   = playerNames[1];
        playerNames[1]   = firstName;

        int firstScore  = playerScores[0];
        playerScores[0] = playerScores[1];
        playerScores[1] = firstScore;

        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();
    }

    private void OnUpdateEnd()
    {
        if (playerScores[currentTeamId] < config.winMax)
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
    public void ResetMatch(bool isStart)
    {
        if (config.playerTypes[0] != PlayerType.Human || config.playerTypes[1] != PlayerType.Human)
            engine = new Engine();
        save = new Save(config.playerTypes);

        playerNames    = new string[2];
        playerNames[0] = config.playerTypes[0] == PlayerType.Human ? "Player" : "AI";
        playerNames[1] = config.playerTypes[1] == PlayerType.Human ? "Player" : "AI";

        if (config.playerTypes[0] == config.playerTypes[1])
        {
            playerNames[0] += " #1";
            playerNames[1] += " #2";
        }

        playerScores  = new int[2];
        currentTeamId = 1;
        board.ResetBoard();
        UI.ResetUI();

        if (isStart)
        {
            OnStateEnter();
            return;
        }

        ChangeState(State.Turn);
    }

    private bool IsWin(Cell cell)
    {
        if (cell.lastPiece.type == PieceType.Wise) return false;
        if (cell.x == 0 && cell.pieces[0].team == 0 || cell.x == board.LineCount - 1 && cell.pieces[0].team == 1)
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

    IEnumerator PlayAuto()
    {
        playAuto = engine.PlayAuto(2);

        yield return null;
    }

    private void AddManualPlay()
    {
        if (config.playerTypes[currentTeamId] != PlayerType.Human || config.playerTypes[1 - currentTeamId] == PlayerType.Human) return;

        int[] manualPlay = new int[6];
        Save.Turn lastTurn = save.turns[save.turns.Count - 1];
        manualPlay[0] = lastTurn.cells[0].x;
        manualPlay[1] = lastTurn.cells[0].y;
        // actions simples
        if (lastTurn.actions.Count < 2)
        {
            if (lastTurn.actions[0] == ActionType.Unstack || lastTurn.actions[0] == ActionType.Stack) // (un)stack
            {
                manualPlay[2] = lastTurn.cells[0].x;
                manualPlay[3] = lastTurn.cells[0].y;
            }
            else // move
            {
                manualPlay[2] = -1;
                manualPlay[3] = -1;
            }
            manualPlay[4] = lastTurn.cells[1].x;
            manualPlay[5] = lastTurn.cells[1].y;

            engine.PlayManual(manualPlay);
            return;
        }

        manualPlay[2] = lastTurn.cells[1].x;
        manualPlay[3] = lastTurn.cells[1].y;
        manualPlay[4] = lastTurn.cells[2].x;
        manualPlay[5] = lastTurn.cells[2].y;

        string text = "";
        foreach (int index in manualPlay)
            text += index;

        engine.PlayManual(manualPlay);
    }
    #endregion
}
