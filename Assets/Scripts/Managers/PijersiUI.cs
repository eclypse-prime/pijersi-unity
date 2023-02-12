using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

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

    private LocalizedStringDatabase stringDatabase;
    private bool isFirstAction = true;
    private List<string> records = new List<string>();

    public Dictionary<string, BetterButton> replayButtons { get; private set; }

    private void Awake()
    {
        stringDatabase = LocalizationSettings.StringDatabase;

        replayButtons = new Dictionary<string, BetterButton>();
        foreach (BetterButton button in replay.GetComponentsInChildren<BetterButton>())
            replayButtons.Add(button.name, button);
    }

    private void Start()
    {
        ResetUI();
    }

    private string GetTeamName(PlayerType teamType, int teamNumber)
    {
        string type = stringDatabase.GetLocalizedString("StaticTexts", teamType.ToString());
        string number = stringDatabase.GetLocalizedString("DynamicTexts", "TeamNumber", arguments: teamNumber);

        return $"{type} {number}";
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
        record.text = "";
        isFirstAction = true;
        records.Clear();
        SetActiveOverlayButtons(true);
        SetReplayButtonsInteractable(false);
    }

    public void ShowEnd(int winTeamId, PlayerType winTeamType, int winTeamNumber, int[] teamWinCounts, int maxWinRound)
    {
        SetActiveOverlayButtons(false);
        end.SetActive(true);
        string teamColor = stringDatabase.GetLocalizedString("DynamicTexts", "TeamColor", arguments: winTeamId);
        endTitle.text = stringDatabase.GetLocalizedString("DynamicTexts", "Winner", arguments: teamColor);

        if (config.isBestOf)
        {
            string score = $"{teamWinCounts[0]} - {teamWinCounts[1]} / {maxWinRound}";
            endScore.text = score;
            pauseScore.text = score;
        }

        if (teamWinCounts[winTeamId] == maxWinRound)
        {
            endResume.interactable = false;
            string score = stringDatabase.GetLocalizedString("DynamicTexts", "BigWinner", arguments: GetTeamName(winTeamType, winTeamNumber));
            endScore.text += $" {score}";
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

    public void SetGameState(int teamId, PlayerType teamType, int teamNumber)
    {
        string color = LocalizationSettings.StringDatabase.GetLocalizedString("DynamicTexts", "TeamColor", arguments: teamId);
        gameState.text = $"{color} : {GetTeamName(teamType, teamNumber)}";
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
        record.text += newRecord;
        isFirstAction = true;
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
