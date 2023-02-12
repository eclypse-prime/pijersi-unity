using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{
    public void Set(BetterButton button)
    {
        Tooltip.Instance.Set(button);
    }

    public void Hide()
    {
        Tooltip.Instance.Hide();
    }
}
