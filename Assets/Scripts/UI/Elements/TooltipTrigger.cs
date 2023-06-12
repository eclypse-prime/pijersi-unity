using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{
    public void Set(BetterButton button) => Tooltip.Set(button);
    public void Hide() => Tooltip.Hide();
}
