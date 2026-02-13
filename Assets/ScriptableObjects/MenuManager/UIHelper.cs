using UnityEngine;
using UnityEngine.UI;

public class UIHelper
{
    public static void SetSelectableObjectState(GameObject rootObject, bool interactable)
    {
        Selectable[] interactiveObjects = rootObject.GetComponentsInChildren<Selectable>();
        foreach (Selectable interactiveObject in interactiveObjects)
            interactiveObject.interactable = interactable;
    }
}
