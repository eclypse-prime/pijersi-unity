using FSM;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class Pijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private PijersiUI UI;
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;
    [SerializeField, Range(0f, 3f)] private float ReplayAndAiDelay;

    // inputs
    [SerializeField] private InputAction mainAction;
    [SerializeField] private InputAction secondaryAction;

    private readonly Dictionary<Cell, Cell[]>[] dangers = new Dictionary<Cell, Cell[]>[2];
    private readonly StateMachine<State> SM = new();

    private new Camera camera;
    private bool isPauseOn;
    private ReplayState replayState;
    private IEngine engine;
    private Save save;
    private Save replaySave;
    private Save loadedSave;
    private Team[] teams;
    private int currentTeamId;
    private Cell pointedCell;
    private Cell lastPointedCell;
    private Cell selectedCell;
    private bool canMove;
    private bool canStack;
    private Dictionary<Cell, List<ActionType>> validMoves;
    private Task<int[]> playAuto;
    private State[] aiActionStates;
    private Cell[] aiActionCells;
    private (int, int) replayAt;
    private float continueAt;
    private ActionType currentAction;

    private Team CurrentTeam => teams[currentTeamId];
    private Team OtherTeam => teams[1 - currentTeamId];

    private struct Team
    {
        public readonly PlayerType type;
        public readonly int number;

        public int score;

        public Team(PlayerType type, int number)
        {
            this.type = type;
            this.number = number;
            score = 0;
        }
    }

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
        Replay,
        AfterReplay
    }

    private enum ReplayState
    {
        None,
        Pause,
        Play
    }

    private void OnEnable()
    {
        mainAction.Enable();
        secondaryAction.Enable();
    }

    private void OnDisable()
    {
        mainAction.Disable();
        secondaryAction.Disable();
    }

    private void Awake() => Init();
    private void Start() => StartLevel();

    private void Update()
    {
        if (isPauseOn) return;

        SM.Update();
    }

    private void Init()
    {
        SM.Add(new State<State>(State.Turn, OnEnterTurn, OnExitTurn, OnUpdateTurn));
        SM.Add(new State<State>(State.PlayerTurn, OnEnterPlayerTurn, null, OnUpdatePlayerTurn));
        SM.Add(new State<State>(State.AiTurn, OnEnterAiTurn, null, OnUpdateAiTurn));
        SM.Add(new State<State>(State.Selection, OnEnterSelection, OnExitSelection, OnUpdateSelection));
        SM.Add(new State<State>(State.PlayAuto, null, onUpdate: OnUpdatePlayAuto));
        SM.Add(new State<State>(State.Move, OnEnterMove, null, OnUpdateMove));
        SM.Add(new State<State>(State.Stack, OnEnterStack, null, OnUpdateStack));
        SM.Add(new State<State>(State.Unstack, OnEnterUnstack, null, OnUpdateUnstack));
        SM.Add(new State<State>(State.End, OnEnterEnd, null, OnUpdateEnd));
        SM.Add(new State<State>(State.Next, OnEnterNext));
        SM.Add(new State<State>(State.Back, OnEnterBack, null, OnUpdateBack));
        SM.Add(new State<State>(State.Replay, null, onUpdate: OnUpdateReplay));
        SM.Add(new State<State>(State.AfterReplay, OnEnterAfterReplay, OnExitAfterReplay));

        camera = Camera.main;
    }

    private void StartLevel()
    {
        cameraMovement.SetCenter(board.Cells[22].Transform.position);

        if (config.partyData != null)
        {
            loadedSave = new Save(board, config.partyData);
            config.playerTypes = loadedSave.playerTypes;
        }

        InitTeams();
        ResetMatch();
    }

    private void InitTeams()
    {
        teams = new Team[2];
        int offset = config.playerTypes[0] == config.playerTypes[1] ? 1 : 0;
        teams[0] = new Team(config.playerTypes[0], 1 * offset);
        teams[1] = new Team(config.playerTypes[1], 2 * offset);
    }

    private void DebugEngine()
    {
        if (engine == null) return;

        Debug.Log(("Engine :\n", engine.ToString()));
        Debug.Log(("Board :\n", board.ToString()));
    }
}
