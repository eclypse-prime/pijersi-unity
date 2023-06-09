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
    [SerializeField] private TextMeshProUGUI[] recordDisplays;
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
    [Header("After Replay Menu :")]
    [SerializeField] private GameObject afterReplay;
    [SerializeField] private TMP_Dropdown[] replayPlayerTypes;
    [SerializeField] private Button replayContinue;


    private LocalizedStringDatabase stringDatabase;
    private bool isFirstAction = true;
    private List<string>[] records = new List<string>[2];

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

    public void ResetUI(int score1, int score2)
    {
        ResetOverlay();

        if (!config.isBestOf)
        {
            endScore.text = pauseScore.text = "";
            return;
        }
        pauseScore.text = $"{score1} - {score2} / {config.winMax}";
    }

    public void ResetUI()
    {
        ResetUI(0, 0);
    }

    public void ResetOverlay()
    {
        recordDisplays[0].text = recordDisplays[1].text = "";
        records[0] = new();
        records[1] = new();
        isFirstAction = true;
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
            endScore.text = pauseScore.text = score;
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

    public void ShowAfterReplayMenu(PlayerType team1, PlayerType team2)
    {
        afterReplay.SetActive(true);
        replayPlayerTypes[0].value = (int)team1;
        replayPlayerTypes[1].value = (int)team2;
        replayContinue.Select();
    }

    public PlayerType[] GetReplayPlayerTypes()
    {
        return new PlayerType[] { (PlayerType)replayPlayerTypes[0].value, (PlayerType)replayPlayerTypes[1].value };
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

    public void UpdateRecord(int teamId, Cell start, Cell end, ActionType action)
    {
        string newRecord = isFirstAction ? start.name : "";

        if (action == ActionType.Move || action == ActionType.Attack)
            newRecord += end.isFull ? stackMoveSign : moveSign;
        else
            newRecord += moveSign;

        newRecord += end.name;

        if (action == ActionType.Attack)
            newRecord += attackSign;

        recordDisplays[teamId].text  += newRecord;
        isFirstAction = false;
        records[teamId].Add(newRecord);
    }

    public void AddRecordColumnLine(int teamId)
    {
        isFirstAction = true;

        if (recordDisplays[teamId].text.Length == 0) return;

        recordDisplays[teamId].text += "\n";
        records[teamId][^1] += "\n";
    }

    public void UndoRecord(int teamId, bool isFirstAction)
    {
        this.isFirstAction = isFirstAction;
        int newRecordSize = records[teamId].Count - 1;
        records[teamId].RemoveAt(newRecordSize);

        string newRecord = "";
        foreach (string record in records[teamId])
            newRecord += record;

        recordDisplays[teamId].text = newRecord;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Tooltip.Hide();
        GameManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        GameManager.Quit();
    }
}
