using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class Tooltip : MonoBehaviour
{
    private const float displayDelay = .5f;
    private const int positionOffset = 11;

    [SerializeField] private RectTransform panel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Animator animator;

    private static Tooltip instance;
    private readonly int showTrigger = Animator.StringToHash("Show");

    private RectTransform canvas;
    private BetterButton currentButton;
    private LocalizedStringDatabase stringDatabase;
    private float showAt = Mathf.Infinity;

    public static void Set(string key, float delay = displayDelay) => instance.LocalSet(key, delay);
    public static void Set(BetterButton button) => instance.LocalSet(button);
    public static void Hide() => instance.LocalHide();


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        canvas = GetComponent<RectTransform>();
        stringDatabase = LocalizationSettings.StringDatabase;

        instance = this;

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
        position.x += positionOffset;
        
        Vector2 pivot = Vector2.up;
        Vector2 scale = canvas.localScale;
        if (position.x + panel.rect.width * scale.x > canvas.rect.width * scale.x)
        {
            position.x -= positionOffset;
            pivot.x = 1f;
        }
        if (position.y - panel.rect.height * scale.y < 0)
            pivot.y = 0f;

        panel.position = position;
        panel.pivot = pivot;
    }

    private void LocalSet(string key, float delay)
    {
        panel.gameObject.SetActive(false);
        showAt = Time.unscaledTime + delay;

        text.text = stringDatabase.GetLocalizedString("Tooltip", key);
    }

    private void LocalSet(BetterButton button)
    {
        currentButton = button;
        
        Set(button.name);
    }

    private void LocalHide()
    {
        panel.gameObject.SetActive(false);
        showAt = Mathf.Infinity;
        currentButton = null;
    }
}
