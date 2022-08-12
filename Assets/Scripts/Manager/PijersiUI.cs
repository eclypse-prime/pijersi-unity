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
    [SerializeField] private TextMeshProUGUI record;

    private bool isLastColumn;

    private void Start()
    {
        ResetUI();
    }

    public void SetActivePause(bool value)
    {
        pause.SetActive(value);
    }

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

        if (!isNewturn)
        {
            newRecord += isLastColumn ? "\n" : "\t";
            isLastColumn = !isLastColumn;
        }

        record.text = record.text + newRecord;
    }

    public void UpdateRecord(int teamId)
    {
        string teamName = teamId == 0 ? "White" : "Black";
        record.text += $"\n{teamName} Win !";
    }

    public void ResetUI()
    {
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
