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
        switch (action)
        {
            case ActionType.none:
                break;
            case ActionType.move:
                newRecord += end.isFull ? stackMoveSign : moveSign;
                newRecord += end.name;
                break;
            case ActionType.attack:
                newRecord += end.isFull ? stackMoveSign : moveSign;
                newRecord += end.name + attackSign;
                break;
            case ActionType.stack:
            case ActionType.unstack:
                newRecord += moveSign + end.name;
                break;
            default:
                break;
        }

        if (!isNewturn)
            newRecord += "\n";

        record.text = record.text + newRecord;
    }

    public void ResetUI()
    {
        record.text = "";
    }
}
