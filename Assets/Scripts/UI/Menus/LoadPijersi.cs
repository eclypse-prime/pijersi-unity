using UnityEngine;

public class LoadPijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;

    public void StartPijersi(SaveButton saveButton)
    {
        config.playerTypes = new PlayerType[] { PlayerType.Human, PlayerType.Human };
        config.partyData = saveButton.SaveName;

        Tooltip.Instance.Hide();
        GameManager.LoadScene("Pijersi");
    }
}
