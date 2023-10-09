using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetterSlider : Slider
{
    protected override void OnEnable()
    {
        base.OnEnable();

        SetValueWithoutNotify(maxValue);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            eventData.button = PointerEventData.InputButton.Left;

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            eventData.button = PointerEventData.InputButton.Left;

        base.OnDrag(eventData);
    }
}
