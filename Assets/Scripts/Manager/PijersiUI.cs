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

    public void SetPauseMenu(bool value)
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

    public void ResetUI()
    {
        record.text = "";
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
