using TMPro;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] public BetterButton button { get; private set; }
    [SerializeField] private TMP_Text saveName;
    [SerializeField] private TMP_Text saveDate;

    public string SaveName => saveName.text;

    public void SetData(string name, string date)
    {
        saveName.text = name;
        saveDate.text = date;
    }
}
