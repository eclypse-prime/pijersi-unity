using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ReplayInput : MonoBehaviour
{
    [SerializeField] private PijersiUI UI;

    private void Update()
    {
        if (UI.replayButtons["Back"].interactable && Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            UI.replayButtons["Back"].onRightClick.Invoke();
        }
    }
}
