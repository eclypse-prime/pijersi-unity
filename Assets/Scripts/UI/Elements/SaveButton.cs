using TMPro;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] public BetterButton button { get; private set; }
    [SerializeField] private TMP_Text saveName;
    [SerializeField] private TMP_Text saveDate;

    public string SaveFullName { get; private set; }

    public void SetData(string name, string fullName, string date)
    {
        saveName.text = name;
        SaveFullName = fullName;
        saveDate.text = date;
    }
}
