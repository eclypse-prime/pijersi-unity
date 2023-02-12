using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class Tooltip : MonoBehaviour
{
    private const float DisplayDelay = 1f;
    private const int positionOffset = 11;

    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;
    private RectTransform canvas;
    private BetterButton currentButton;
    private float showAt = Mathf.Infinity;
    private int showTrigger = Animator.StringToHash("Show");
    private LocalizedStringDatabase stringDatabase;

    public static Tooltip Instance { get; private set; }

    private void Awake()
    {
        canvas = GetComponent<RectTransform>();
        stringDatabase = LocalizationSettings.StringDatabase;

        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (showAt <= Time.unscaledTime)
        {
            Show();
            return;
        }

        if (currentButton == null) return;

        if (!currentButton.isActiveAndEnabled || !currentButton.interactable)
            Hide();
    }

    private void Show()
    {
        panel.gameObject.SetActive(true);
        showAt = Mathf.Infinity;
        SetPosition();
        animator.SetTrigger(showTrigger);
    }

    private void SetPosition()
    {
        Vector3 position = Mouse.current.position.ReadValue();
        position.x = position.x + positionOffset;
        
        Vector2 pivot = Vector2.up;
        Vector2 scale = canvas.localScale;
        if (position.x + panel.rect.width * scale.x > canvas.rect.width * scale.x)
        {
            position.x = position.x - positionOffset;
            pivot.x = 1f;
        }
        if (position.y - panel.rect.height * scale.y < 0)
            pivot.y = 0f;

        panel.position = position;
        panel.pivot = pivot;
    }

    public void Set(string key, float delay = DisplayDelay)
    {
        panel.gameObject.SetActive(false);
        showAt = Time.unscaledTime + delay;

        text.text = stringDatabase.GetLocalizedString("Tooltip", key);
    }

    public void Set(BetterButton button)
    {
        currentButton = button;
        
        Set(button.name);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
        showAt = Mathf.Infinity;
        currentButton = null;
    }
}
