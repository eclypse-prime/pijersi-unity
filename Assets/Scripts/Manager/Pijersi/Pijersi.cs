using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.InputSystem;

public partial class Pijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private PijersiUI UI;
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;

    private new Camera camera;
    private bool isPauseOn;
    private ReplayState replayState;
    private ReplayType replayType;
    private IEngine engine;
    private Save save;
    private Save replaySave;
    private Cell pointedCell;
    private Cell selectedCell;
    private int currentTeamId;
    private bool canMove;
    private bool canStack;
    private Dictionary<Cell, List<ActionType>> validMoves;
    private Dictionary<Cell, List<Cell>> dangers;
    private int[] playerScores;
    private string[] playerNames;
    private int[] playAuto;
    private State[] aiActionStates;
    private Cell[] aiActionCells;

    private StateMachine<State> SM = new StateMachine<State>();

    private enum State
    {
        Turn,
        PlayerTurn,
        AiTurn,
        Selection,
        PlayAuto,
        Move,
        Stack,
        Unstack,
        End,
        Back,
        Next,
        Replay
    }

    private enum ReplayState
    {
        None,
        Pause,
        Play
    }

    private enum ReplayType
    {
        Action,
        Turn
    }

    private void Awake()
    {
        SM.Add(new State<State>(State.Turn, OnEnterTurn, OnExitTurn, OnUpdateTurn));
        SM.Add(new State<State>(State.PlayerTurn, OnEnterPlayerTurn, OnExitPlayerTurn, OnUpdatePlayerTurn));
        SM.Add(new State<State>(State.AiTurn, OnEnterAiTurn, OnExitAiTurn, OnUpdateAiTurn));
        SM.Add(new State<State>(State.Selection, OnEnterSelection, OnExitSelection, OnUpdateSelection));
        SM.Add(new State<State>(State.PlayAuto, OnEnterPlayAuto, OnExitPlayAuto, OnUpdatePlayAuto));
        SM.Add(new State<State>(State.Move, OnEnterMove, OnExitMove, OnUpdateMove));
        SM.Add(new State<State>(State.Stack, OnEnterStack, OnExitStack, OnUpdateStack));
        SM.Add(new State<State>(State.Unstack, OnEnterUnstack, OnExitUnstack, OnUpdateUnstack));
        SM.Add(new State<State>(State.End, OnEnterEnd, OnExitEnd, OnUpdateEnd));
        SM.Add(new State<State>(State.Next, OnEnterNext, null, OnUpdateNext));
        SM.Add(new State<State>(State.Back, OnEnterBack, null, OnUpdateBack));
        SM.Add(new State<State>(State.Replay, OnEnterReplay, OnExitReplay, OnUpdateReplay));

        camera = Camera.main;
    }

    private void Start()
    {
        cameraMovement.SetCenter(board.cells[22].transform.position);
        ResetMatch();
    }

    private void Update()
    {
        if (CheckPause()) return;

        SM.Update();
    }

    private bool CheckPause()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();

        return isPauseOn;
    }
}
