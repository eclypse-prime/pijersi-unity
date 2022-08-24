using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public partial class Pijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private PijersiUI UI;
    [SerializeField] private Board board;
    [SerializeField] private new BoardAnimation animation;
    [SerializeField] private LayerMask cellLayer;

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
    }

    private void Awake()
    {
        SM.Add(new State<State>(State.Turn, "Turn", OnEnterTurn, OnExitTurn, OnUpdateTurn));
        SM.Add(new State<State>(State.PlayerTurn, "PlayerTurn", OnEnterPlayerTurn, OnExitPlayerTurn, OnUpdatePlayerTurn));
        SM.Add(new State<State>(State.AiTurn, "AiTurn", OnEnterAiTurn, OnExitAiTurn, OnUpdateAiTurn));
        SM.Add(new State<State>(State.Selection, "Selection", OnEnterSelection, OnExitSelection, OnUpdateSelection));
        SM.Add(new State<State>(State.PlayAuto, "PlayAuto", OnEnterPlayAuto, OnExitPlayAuto, OnUpdatePlayAuto));
        SM.Add(new State<State>(State.Move, "Move", OnEnterMove, OnExitMove, OnUpdateMove));
        SM.Add(new State<State>(State.Stack, "Stack", OnEnterStack, OnExitStack, OnUpdateStack));
        SM.Add(new State<State>(State.Unstack, "Unstack", OnEnterUnstack, OnExitUnstack, OnUpdateUnstack));
        SM.Add(new State<State>(State.End, "End", OnEnterEnd, OnExitEnd, OnUpdateEnd));

        camera = Camera.main;
    }

    private void Start()
    {
        ResetMatch(true);
    }

    private void Update()
    {
        if (CheckPause()) return;

        SM.Update();
    }
}
