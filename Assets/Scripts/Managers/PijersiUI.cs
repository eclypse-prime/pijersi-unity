using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class PijersiUI : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private Overlay overlay;
    [SerializeField] private Pause pause;
    [SerializeField] private End end;
    [SerializeField] private AfterReplay afterReplay;

    private LocalizedStringDatabase stringDatabase;

    public Dictionary<string, BetterButton> ReplayButtons => overlay.ReplayButtons;

    private void Awake()
    {
        stringDatabase = LocalizationSettings.StringDatabase;
    }

    private void Start() => ResetUI();

    public void ResetUI(int score1, int score2)
    {
        overlay.DoReset();

        if (!config.IsBestOf)
        {
            end.Score.text = pause.Score.text = "";
            return;
        }
        pause.Score.text = $"{score1} - {score2} / {config.winMax}";
    }

    public void ResetUI() => ResetUI(0, 0);
    public void ResetOverlay() => overlay.DoReset();

    public void SetGameState(int teamId, PlayerType teamType, int teamNumber) =>
        overlay.SetGameState(teamId, GetTeamName(teamType, teamNumber));

    public PlayerType[] GetReplayPlayerTypes() =>
        afterReplay.GetPlayerTypes();

    public void ShowAfterReplayMenu(PlayerType team, PlayerType team1) =>
        afterReplay.Show(team, team1);

    public void ShowEnd(int winTeamId, PlayerType winTeamType, int winTeamNumber, int[] teamWinCounts, int maxWinRound)
    {
        overlay.SetActiveAllButtons(false);

        if (config.IsBestOf)
        {
            string score = $"{teamWinCounts[0]} - {teamWinCounts[1]} / {maxWinRound}";
            end.Score.text = pause.Score.text = score;
        }

        string teamName = GetTeamName(winTeamType, winTeamNumber);
        end.Show(winTeamId, teamName, teamWinCounts, maxWinRound);
    }

    public void UpdateRecord(int teamId, Cell start, Cell end, ActionType action) =>
        overlay.UpdateRecord(teamId, start, end, action);

    public void AddRecordColumnLine(int teamId) =>
        overlay.AddRecordColumnLine(teamId);

    public void UndoRecord(int teamId, bool isFirstAction) =>
        overlay.UndoRecord(teamId, isFirstAction);

    public void MainMenu()
    {
        Time.timeScale = 1f;
        Tooltip.Hide();
        GameManager.LoadScene("Start");
    }

    public void QuitGame() => GameManager.Quit();

    private string GetTeamName(PlayerType teamType, int teamNumber)
    {
        string type = stringDatabase.GetLocalizedString("StaticTexts", teamType.ToString());
        string number = stringDatabase.GetLocalizedString("DynamicTexts", "TeamNumber", arguments: teamNumber);

        return $"{type} {number}";
    }
}
