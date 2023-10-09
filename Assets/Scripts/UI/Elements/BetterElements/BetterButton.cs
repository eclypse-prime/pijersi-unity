using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

[AddComponentMenu("UI/BetterButton", 30)]
public class BetterButton : Button
{
    [FormerlySerializedAs("onRightClick")]
    [SerializeField] private ButtonClickedEvent m_onRightClick = new();
    [SerializeField] private InputAction pressAction;
    [SerializeField] private InputAction alternativePressAction;

    public ButtonClickedEvent onRightClick
    {
        get { return m_onRightClick; }
        set { m_onRightClick = value; }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        pressAction.Enable();
        alternativePressAction.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        pressAction.Disable();
        alternativePressAction.Disable();
    }

    private void Update()
    {
        if (!IsActive() || !IsInteractable() || !pressAction.triggered)
            return;

        if (alternativePressAction.inProgress)
        {
            onRightClick.Invoke();
            return;
        }

        onClick.Invoke();
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onRightClick", this);

        if (onRightClick.GetPersistentEventCount() == 0)
        {
            onClick.Invoke();
            return;
        }

        m_onRightClick.Invoke();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        Press();
    }

    public override void OnPointerDown(PointerEventData eventData) =>
        OnPointerLeftOrRight(eventData, base.OnPointerUp);

    public override void OnPointerUp(PointerEventData eventData) =>
        OnPointerLeftOrRight(eventData, base.OnPointerUp);

    // Reload inputActions (for inputs localization)
    public void ReloadInputAction()
    {
        pressAction.Disable();
        alternativePressAction.Disable();
        pressAction.Dispose();
        alternativePressAction.Dispose();
        pressAction.Enable();
        alternativePressAction.Enable();
    }

    private void OnPointerLeftOrRight(PointerEventData eventData, Action<PointerEventData> baseAction)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.button = PointerEventData.InputButton.Left;
            baseAction(eventData);
            eventData.button = PointerEventData.InputButton.Right;
            return;
        }

        baseAction(eventData);
    }
}
