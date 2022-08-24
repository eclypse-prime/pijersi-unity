using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PijersiUI : MonoBehaviour
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';

    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject resume;
    [SerializeField] private Button save;
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Display;
    [SerializeField] private TextMeshProUGUI gameState;
    [SerializeField] private TextMeshProUGUI record;

    private string[] teamColor = {"White", "Black"};
    private bool isFirstAction = true;

    private void Start()
    {
        ResetUI();
    }

    public void SetActivePause(bool value)
    {
        pause.SetActive(value);
    }

    public void ShowEnd(int winTeamId, int[] teamWinCounts, int maxWinRound)
    {
        pause.SetActive(true);
        save.gameObject.SetActive(true);
        if (teamWinCounts[winTeamId] == maxWinRound)
            resume.SetActive(false);
        Title.text = $"{teamColor[winTeamId]} win !";
        Display.text = $"{teamWinCounts[0]} - {teamWinCounts[1]} / {maxWinRound}";
    }

    public void HideEnd()
    {
        pause.SetActive(false);
        save.gameObject.SetActive(false);
        save.interactable = true;
        resume.SetActive(true);
        Title.text = "Pause";
        Display.text = "";
    }

    public void UpdateGameState(int teamId, string teamName)
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

        record.text += newRecord;
        isFirstAction = false;
    }

    public void AddRecordColumnLine(int teamId)
    {
        if (record.text.Length == 0) return;

        record.text += teamId == 0 ? "\n" : "\t";
        isFirstAction = true;
    }

    public void ResetUI()
    {
        gameState.text = "";
        record.text = "";
        isFirstAction = true;
        HideEnd();
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
