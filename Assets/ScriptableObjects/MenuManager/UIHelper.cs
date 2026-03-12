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
    public static bool FindComponentInObjects<T>(GameObject[] objects, out T targetComponent)
    {
        targetComponent = default;
        foreach (GameObject gameObject in objects)
            if (gameObject.TryGetComponent(out targetComponent))
                return true;

        return false;
    }
}