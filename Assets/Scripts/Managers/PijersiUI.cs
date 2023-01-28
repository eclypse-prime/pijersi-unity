using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PijersiUI : MonoBehaviour
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';

    [SerializeField] private PijersiConfig config;
    [Header("Overlay :")]
    [SerializeField] private TextMeshProUGUI gameState;
    [SerializeField] private TextMeshProUGUI record;
    [SerializeField] private GameObject replay;
    [SerializeField] private Button[] overlayButtons;
    [Header("Pause Menu :")]
    [SerializeField] private GameObject pause;
    [SerializeField] private TextMeshProUGUI pauseScore;
    [SerializeField] private Button pauseResume;
    [Header("End Menu :")]
    [SerializeField] private GameObject end;
    [SerializeField] private TextMeshProUGUI endTitle;
    [SerializeField] private TextMeshProUGUI endScore;
    [SerializeField] private Button endResume;
    [SerializeField] private Button save;

    private string[] teamColor = {"White", "Black"};
    private bool isFirstAction = true;
    private List<string> records = new List<string>();

    public Dictionary<string, BetterButton> replayButtons { get; private set; }

    private void Awake()
    {
        replayButtons = new Dictionary<string, BetterButton>();
        foreach (BetterButton button in replay.GetComponentsInChildren<BetterButton>())
            replayButtons.Add(button.name, button);
    }

    private void Start()
    {
        ResetUI();
    }

    public void ResetUI((int, int) scores)
    {
        ResetOverlay();

        if (!config.isBestOf)
        {
            endScore.text = "";
            pauseScore.text = "";
            return;
        }
        pauseScore.text = $"{scores.Item1} - {scores.Item2} / {config.winMax}";
    }

    public void ResetUI()
    {
        ResetUI((0, 0));
    }

    public void ResetOverlay()
    {
        gameState.text = "";
        record.text = "";
        isFirstAction = true;
        records.Clear();
        SetActiveOverlayButtons(true);
        SetReplayButtonsInteractable(false);
    }

    public void ShowEnd(int winTeamId, int[] teamWinCounts, int maxWinRound)
    {
        SetActiveOverlayButtons(false);
        end.SetActive(true);
        endTitle.text   = $"{teamColor[winTeamId]} win !";

        if (config.isBestOf)
        {
            string score = $"{teamWinCounts[0]} - {teamWinCounts[1]} / {maxWinRound}";
            endScore.text = score;
            pauseScore.text = score;
        }

        if (teamWinCounts[winTeamId] == maxWinRound)
        {
            endResume.gameObject.SetActive(false);
            save.Select();

            return;
        }

        endResume.Select();
    }

    public void SetActiveOverlayButtons(bool value)
    {
        foreach (Selectable button in overlayButtons)
            button.enabled = value;
    }

    public void SetReplayButtonsInteractable(bool value)
    {
        foreach (KeyValuePair<string, BetterButton> replayButton in replayButtons)
            replayButton.Value.interactable = value;
    }

    public void SetGameState(int teamId, string teamName)
    {
        gameState.text = (teamId == 0 ? "white" : "black") + " : " + teamName;
    }

    public void UpdateRecord(Cell start, Cell end, ActionType action)
    {
        string newRecord = isFirstAction ? start.name : "";

        if (action == ActionType.Move || action == ActionType.Attack)
            newRecord += end.isFull ? stackMoveSign : moveSign;
        else
            newRecord += moveSign;

        newRecord += end.name;

        if (action == ActionType.Attack)
            newRecord += attackSign;

        record.text  += newRecord;
        isFirstAction = false;
        records.Add(newRecord);
    }

    public void AddRecordColumnLine(int teamId)
    {
        if (record.text.Length == 0) return;

        string newRecord = teamId == 0 ? "\n" : "\t";
        record.text     += newRecord;
        isFirstAction    = true;
        records[records.Count -1] += newRecord;
    }

    public void UndoRecord()
    {
        int newRecordSize = records.Count - 1;
        records.RemoveAt(newRecordSize);
        if (newRecordSize == 0)
            isFirstAction = true;

        string newRecord = "";
        foreach (string record in records)
            newRecord += record;

        record.text = newRecord;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        GameManager.Quit();
    }
}
