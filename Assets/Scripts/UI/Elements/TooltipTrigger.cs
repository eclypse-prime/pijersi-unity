using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{
    public void Show(BetterButton button)
    {
        Tooltip.Instance.Show(button);
    }

    public void Hide()
    {
        Tooltip.Instance.Hide();
    }
}
