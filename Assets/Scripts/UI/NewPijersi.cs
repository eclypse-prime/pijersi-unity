using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewPijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;
    [SerializeField] private Slider winMax;
    [SerializeField] private TMP_Dropdown[] PlayerTypes;

    public void StartPijersi()
    {
        config.playerTypes = new PlayerType[] { (PlayerType) PlayerTypes[0].value, (PlayerType) PlayerTypes[1].value };
        config.winMax = (int) winMax.value;
        config.partyData = null;

        GameManager.LoadScene("Pijersi");
    }
}
