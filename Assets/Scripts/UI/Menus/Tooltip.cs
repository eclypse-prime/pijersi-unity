using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform canvas;

    public static Tooltip Instance { get; private set; }

    private void Awake()
    {
        canvas = GetComponent<RectTransform>();

        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void Show(string content)
    {
        panel.gameObject.SetActive(true);
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        panel.position = mousePosition;

        Vector2 pivot = Vector2.up;
        Vector2 scale = canvas.localScale;
        if (mousePosition.x + panel.rect.width * scale.x > canvas.rect.width * scale.x)
            pivot.x = 1f;
        if (mousePosition.y - panel.rect.height * scale.y < 0)
            pivot.y = 0f;
        panel.pivot = pivot;

        text.text = content;
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}
