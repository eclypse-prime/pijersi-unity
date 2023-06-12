using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class End : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button resume;
    [SerializeField] private Button save;

    private LocalizedStringDatabase stringDatabase;

    public TextMeshProUGUI Title => title;
    public TextMeshProUGUI Score => score;
    public Button Resume => resume;
    public Button Save => save;

    private void Awake()
    {
        stringDatabase = LocalizationSettings.StringDatabase;
    }

    public void Show(int winTeamId, string teamName, int[] teamWinCounts, int maxWinRound)
    {
        gameObject.SetActive(true);
        string teamColor = stringDatabase.GetLocalizedString("DynamicTexts", "TeamColor", arguments: winTeamId);
        Title.text = stringDatabase.GetLocalizedString("DynamicTexts", "Winner", arguments: teamColor);

        if (teamWinCounts[winTeamId] == maxWinRound)
        {
            Resume.interactable = false;
            string score = stringDatabase.GetLocalizedString("DynamicTexts", "BigWinner", arguments: teamName);
            Score.text += $" {score}";
            Save.Select();

            return;
        }

        Resume.Select();
    }
}
