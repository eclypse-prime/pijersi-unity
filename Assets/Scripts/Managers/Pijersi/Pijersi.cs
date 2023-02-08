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

    // inputs
    [SerializeField] private InputAction mainAction;
    [SerializeField] private InputAction secondaryAction;

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
    private List<Cell>[] selectedCellDangers = new List<Cell>[2];
    private Dictionary<Cell, List<Cell>> dangers;
    private int[] playAuto;
    private State[] aiActionStates;
    private Cell[] aiActionCells;
    private (int, int) replayAt;

    private StateMachine<State> SM = new StateMachine<State>();

    private struct Team
    {
        private PlayerType type;
        private string name;
        public int score;

        public PlayerType Type => type;
        public string Name => name;

        public Team(PlayerType type, string name)
        {
            this.type = type;
            this.name = name;
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

        string[] teamNames = new string[2];
        for (int i = 0; i < 2; i++)
        {
            switch (config.playerTypes[i])
            {
                case PlayerType.Human:
                    teamNames[i] = "Player";
                    break;
                case PlayerType.AiEasy:
                    teamNames[i] = "AI (easy)";
                    break;
                case PlayerType.AiNormal:
                    teamNames[i] = "AI (normal)";
                    break;
                case PlayerType.AiHard:
                    teamNames[i] = "AI (hard)";
                    break;
                case PlayerType.AiInsane:
                    teamNames[i] = "AI (insane)";
                    break;
                case PlayerType.AiGod:
                    teamNames[i] = "AI (god)";
                    break;
                default:
                    break;
            }
        }

        if (config.playerTypes[0] == config.playerTypes[1])
        {
            teamNames[0] += " #1";
            teamNames[1] += " #2";
        }

        teams[0] = new Team(config.playerTypes[0], teamNames[0]);
        teams[1] = new Team(config.playerTypes[1], teamNames[1]);
    }
}
