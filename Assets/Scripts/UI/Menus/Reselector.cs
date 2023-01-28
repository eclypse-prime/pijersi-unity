using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Reselector : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    
    private GameObject lastSelectedObject;

    private void Update()
    {
        TryToReselected();
    }

    private void TryToReselected()
    {
        GameObject currentSelectedObject = eventSystem.currentSelectedGameObject;

        if (currentSelectedObject == lastSelectedObject) return;

        if (lastSelectedObject == null || currentSelectedObject != null)
        {
            lastSelectedObject = currentSelectedObject;
            return;
        }

        lastSelectedObject.GetComponent<Selectable>().Select();
    }
}
