using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ReplayMenu : MonoBehaviour
{
    [SerializeField] private Dictionary<ReplayType, Toggle> replayButtons;

    private ToggleGroup group;

    public ReplayType selection;

    private void Awake()
    {
        group = GetComponent<ToggleGroup>();
    }

    public void SetSelection(ReplayType replayType)
    {
        if (replayType == ReplayType.None || replayType == selection)
        {
            selection = ReplayType.None;
            group.SetAllTogglesOff();

            return;
        }

        selection = replayType;
        replayButtons[replayType].isOn = true;
    }
}
