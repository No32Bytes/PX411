using System.Collections.Generic;
using UnityEngine;

public class EntityInformationView : MonoBehaviour
{
    public static EntityInformationView Current { get; private set; } = null;
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
    public GameObject SourceGameObject => sourceGameObject;
    [SerializeField] private GameObject[] informationObjectPrefab;
    private readonly List<GameObject> informationObjectInstance = new();
    public void Show()
    {
        if (informationObjectInstance.Count != 0)
            return;

        if (sourceGameObject == null)
            sourceGameObject = gameObject;

        GameObject infoView = GlobalDataStore.GetStateManager().playerState.playerRef.overlayInformationView;
        if (infoView == null)
            return;
        if (informationObjectPrefab.Length == 0)
            return;


        foreach (GameObject gameObject in informationObjectPrefab)
        {
            GameObject newObject = Instantiate(gameObject, infoView.transform);
            if (newObject == null)
                continue;
            informationObjectInstance.Add(newObject);
        }


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
    }
}