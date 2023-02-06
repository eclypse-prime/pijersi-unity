using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform textTransform;
    private RectTransform canvas;

    public static Tooltip Instance { get; private set; }

    private void Awake()
    {
        textTransform = text.rectTransform;
        canvas = GetComponent<RectTransform>();

        Instance = this;

        DontDestroyOnLoad(this);
    }

    public void Show(string content)
    {
        textTransform.gameObject.SetActive(true);
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        textTransform.position = mousePosition;

        Vector2 pivot = Vector2.up;
        Vector2 scale = canvas.localScale;
        if (mousePosition.x + textTransform.rect.width * scale.x > canvas.rect.width * scale.x)
            pivot.x = 1f;
        if (mousePosition.y - textTransform.rect.height * scale.y < 0)
            pivot.y = 0f;
        textTransform.pivot = pivot;
        Debug.Log((mousePosition, textTransform.rect.width, canvas.rect.width));
        text.text = content;
    }

    public void Hide()
    {
        textTransform.gameObject.SetActive(false);
    }
}
