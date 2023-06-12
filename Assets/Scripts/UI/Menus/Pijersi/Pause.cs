using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class Pause : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button resume;

    public TextMeshProUGUI Score => score;
    public Button Resume => resume;
}
