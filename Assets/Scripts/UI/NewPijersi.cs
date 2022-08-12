using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewPijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private TMP_Dropdown typeSelect;
    [SerializeField] private TMP_Dropdown teamSelect;

    public int Type
    {
        set
        {
            UpdateOptions();
        }
    }

    private void UpdateOptions()
    {
        teamSelect.interactable = (GameType) typeSelect.value != GameType.PlayerVsPlayer;
    }

    public void StartPijersi()
    {
        config.gameType = (GameType) typeSelect.value;
        config.playerId = teamSelect.value;
        GameManager.LoadScene("Pijersi");
    }
}
