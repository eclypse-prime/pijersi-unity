using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

[DisallowMultipleComponent]
public class Overlay : MonoBehaviour
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';

    [SerializeField] private TextMeshProUGUI gameState;
    [SerializeField] private TextMeshProUGUI[] recordDisplays;
    [SerializeField] private GameObject replay;
    [SerializeField] private Button[] overlayButtons;

    private bool isFirstAction = true;
    private readonly List<string>[] records = new List<string>[2];

    public Dictionary<string, BetterButton> ReplayButtons { get; private set; }

    private void Awake()
    {
        ReplayButtons = new Dictionary<string, BetterButton>();
        foreach (BetterButton button in replay.GetComponentsInChildren<BetterButton>())
            ReplayButtons.Add(button.name, button);
    }

    public void DoReset()
    {
        recordDisplays[0].text = recordDisplays[1].text = "";
        records[0] = new();
        records[1] = new();
        isFirstAction = true;
        SetActiveAllButtons(true);
        SetReplayButtonsInteractable(false);
    }

    public void SetActiveAllButtons(bool value)
    {
        foreach (Selectable button in overlayButtons)
            button.enabled = value;
    }

    public void SetReplayButtonsInteractable(bool value)
    {
        foreach (KeyValuePair<string, BetterButton> replayButton in ReplayButtons)
            replayButton.Value.interactable = value;
    }

    public void SetGameState(int teamId, string teamName)
    {
        string color = LocalizationSettings.StringDatabase.GetLocalizedString("DynamicTexts", "TeamColor", arguments: teamId);
        gameState.text = $"{color} : {teamName}";
    }

    public void UpdateRecord(int teamId, Cell start, Cell end, ActionType action)
    {
        string newRecord = isFirstAction ? start.name : "";

        if (action == ActionType.Move || action == ActionType.Attack)
            newRecord += end.IsFull ? stackMoveSign : moveSign;
        else
            newRecord += moveSign;

        newRecord += end.name;
        newRecord += action == ActionType.Attack ? attackSign : " ";

        recordDisplays[teamId].text += newRecord;
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
}
