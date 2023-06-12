using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class AfterReplay : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown[] playerTypes;
    [SerializeField] private Button resume;

    public void Show(PlayerType team1, PlayerType team2)
    {
        gameObject.SetActive(true);
        playerTypes[0].value = (int)team1;
        playerTypes[1].value = (int)team2;
        resume.Select();
    }

    public PlayerType[] GetPlayerTypes() =>
        new PlayerType[] { (PlayerType)playerTypes[0].value, (PlayerType)playerTypes[1].value };
}
