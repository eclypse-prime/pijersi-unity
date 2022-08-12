using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PijersiUI : MonoBehaviour
{
    private const char moveSign = '-';
    private const char stackMoveSign = '=';
    private const char attackSign = '!';

    [SerializeField] private GameObject pause;
    [SerializeField] private TextMeshProUGUI gameState;
    [SerializeField] private TextMeshProUGUI record;

    private bool isLastColumn;

    #region base
    private void Start()
    {
        ResetUI();
    }
    #endregion

    public void SetActivePause(bool value)
    {
        pause.SetActive(value);
    }

    #region game state
    public void UpdateGameState(int teamId, bool isAi)
    {
        gameState.text = (teamId == 0 ? "white" : "black") + (isAi ? " (AI)" : "");
    }

    public void UpdateGameState()
    {
        gameState.text += " win !";
    }
    #endregion

    #region record
    public void UpdateRecord(Cell start, Cell end, ActionType action, bool isNewturn)
    {
        string newRecord = isNewturn ? start.name : "";

        if (action == ActionType.Move || action == ActionType.Attack)
            newRecord += end.isFull ? stackMoveSign : moveSign;
        else
            newRecord += moveSign;

        newRecord += end.name;

        if (action == ActionType.Attack)
            newRecord += attackSign;

        record.text += newRecord;
    }

    public void AddRecordColumnLine(int teamId)
    {
        if (record.text.Length == 0) return;

        record.text += teamId == 0 ? "\n" : "\t";
    }
    #endregion

    public void ResetUI()
    {
        gameState.text = "";
        record.text = "";
    }

    public void MainMenu()
    {
        GameManager.LoadScene("Start");
    }

    public void QuitGame()
    {
        GameManager.Quit();
    }
}
