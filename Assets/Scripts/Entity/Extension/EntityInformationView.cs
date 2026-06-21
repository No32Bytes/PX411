using System.Collections.Generic;
using UnityEngine;

public class EntityInformationView : MonoBehaviour
{
    public static EntityInformationView Current { get; private set; } = null;
    public static void SetInteractInfo(GameObject gameObject, string text, bool enabled = true)
    {
        if (Current == null)
            return;
        Current.SetInteractInfoInternal(gameObject, text, enabled);
    }
    public static void SelectEntity(EntityInformationView entityNew)
    {
        if (Equals(Current, entityNew))
            return;

        if (Current != null)
            Current.Hide();


        Current = entityNew;
        if (entityNew != null)
            Current.Show();
    }
    [SerializeField] private GameObject sourceGameObject;
    [SerializeField] private GameObject[] informationObjectPrefab;
    [System.Serializable]
    public struct InteractionInfo
    {
        public bool hasInfo;
        public string displayText;
    };
    [SerializeField] private InteractionInfo interactionInfo;
    public GameObject SourceGameObject => sourceGameObject;
    private readonly List<GameObject> informationObjectInstance = new();
    private GameObject interactInfoInstance;
    public void Show()
    {
        if (informationObjectInstance.Count != 0)
            return;

        if (sourceGameObject == null)
            sourceGameObject = gameObject;

        GameObject infoView = GlobalDataStore.GetStateManager().playerState.playerRef.overlayInformationView;
        if (infoView == null)
            return;
        if (informationObjectPrefab.Length != 0)
        {
            foreach (GameObject gameObject in informationObjectPrefab)
            {
                GameObject newObject = Instantiate(gameObject, infoView.transform);
                if (newObject == null)
                    continue;
                informationObjectInstance.Add(newObject);
            }
        }

        if (interactionInfo.hasInfo)
        {
            GameObject newObject = Instantiate(GlobalDataStore.Instance.entityInformationViewInteractInfo, infoView.transform);
            if (newObject == null)
                return;
            informationObjectInstance.Add(newObject);
            interactInfoInstance = newObject;
            if (!string.IsNullOrEmpty(interactionInfo.displayText))
                SetInteractInfo(sourceGameObject, interactionInfo.displayText);
            else
                SetInteractInfo(sourceGameObject, "", false);
        }
    }

    private void SetInteractInfoInternal(GameObject gameObject, string text, bool enabled)
    {
        if (!gameObject.Equals(sourceGameObject))
            return;

        if (interactInfoInstance == null)
            return;

        if (!enabled || string.IsNullOrEmpty(text))
        {
            interactInfoInstance.SetActive(false);
            return;
        }

        if (!interactInfoInstance.TryGetComponent<EntityInformationInteractInfo>(out var entityInformationInteractInfo))
            return;

        entityInformationInteractInfo.SetText(text);
        interactInfoInstance.SetActive(true);
    }

    public void Hide()
    {
        if (informationObjectInstance.Count == 0)
            return;

        foreach (GameObject gameObject in informationObjectInstance)
        {
            Destroy(gameObject);
        }
        informationObjectInstance.Clear();
        interactInfoInstance = null;
    }
}