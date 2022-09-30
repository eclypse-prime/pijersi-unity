using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/BetterButton", 30)]
public class BetterButton : Button
{
    [FormerlySerializedAs("onRightClick")]
    [SerializeField] private ButtonClickedEvent m_OnRightClick = new ButtonClickedEvent();

    public ButtonClickedEvent OnRightClick
    {
        get { return m_OnRightClick; }
        set { m_OnRightClick = value; }
    }

    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onRightClick", this);
        m_OnRightClick.Invoke();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        Press();
    }
}
