using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetterToggle : Toggle
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            eventData.button = PointerEventData.InputButton.Left;

        base.OnPointerClick(eventData);
    }
}
