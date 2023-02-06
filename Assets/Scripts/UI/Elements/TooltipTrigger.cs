using UnityEngine;

public class TooltipTrigger : MonoBehaviour
{
    public void Show()
    {
        Tooltip.Instance.Show(name);
    }

    public void Hide()
    {
        Tooltip.Instance.Hide();
    }
}
