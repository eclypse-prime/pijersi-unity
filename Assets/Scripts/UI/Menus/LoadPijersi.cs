using UnityEngine;

public class LoadPijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;

    public void StartPijersi(SaveButton saveButton)
    {
        config.playerTypes = new PlayerType[] { PlayerType.Human, PlayerType.Human };
        config.partyData = saveButton.SaveFullName;

        Tooltip.Hide();
        GameManager.LoadScene("Pijersi");
    }
}
