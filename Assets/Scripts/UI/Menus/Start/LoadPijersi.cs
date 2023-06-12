using UnityEngine;

public class LoadPijersi : MonoBehaviour
{
    [SerializeField] private PijersiConfig config;

    public void StartPijersi(SaveButton saveButton)
    {
        config.playerTypes = new PlayerType[] { PlayerType.Human, PlayerType.Human };
        config.partyData = saveButton.SaveFullName;

        LoadScene();
    }

    public void LoadFromClipboard()
    {
        string buffer = GUIUtility.systemCopyBuffer;

        if (!Save.IsValidData(buffer)) return;

        config.partyData = buffer;
        LoadScene();
    }

    private static void LoadScene()
    {
        Tooltip.Hide();
        GameManager.LoadScene("Pijersi");
    }
}
