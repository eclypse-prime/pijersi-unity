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

    private Team CurrentTeam => teams[currentTeamId];
    private Team OtherTeam => teams[1 - currentTeamId];

    private struct Team
    {
        private PlayerType type;
        private int number;

        public int score;

        public PlayerType Type => type;
        public int Number => number;

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
        Replay
    }

    private enum ReplayState
    {
        None,
        Pause,
        Play
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

    private void Start()
    {
        cameraMovement.SetCenter(board.cells[22].transform.position);
        InitTeams();

        // Dans le cas d'un chargement de partie
        if (config.partyData != null)
        {
            save = new Save(board, config.partyData);
            Replay();
            return;
        }

        ResetMatch();
    }

    private void Update()
    {
        if (isPauseOn) return;

        SM.Update();
    }

    private void InitTeams()
    {
        teams = new Team[2];
        int offset = config.playerTypes[0] == config.playerTypes[1] ? 1 : 0;
        teams[0] = new Team(config.playerTypes[0], 1 * offset);
        teams[1] = new Team(config.playerTypes[1], 2 * offset);
    }
}
