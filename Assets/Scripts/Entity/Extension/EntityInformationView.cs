

using UnityEngine;

public class EntityInformationView : MonoBehaviour
{
    public static EntityInformationView Current { get; private set; } = null;
    public static void SelectEntity(EntityInformationView entityNew)
    {
        if (Current == entityNew)
            return;

        if (Current != null)
            Current.Hide();


        Current = entityNew;
        if (entityNew != null)
            Current.Show();
    }

    [SerializeField] private GameObject informationObjectPrefab;
    [SerializeField] private Vector3 informationPosition = new();
    private GameObject informationObjectInstance;
    public void Show()
    {
        if (informationObjectInstance != null)
            return;

        GameObject infoView = GlobalDataStore.GetStateManager().playerState.playerRef.overlayInformationView;
        if (infoView == null)
            return;
        if (informationObjectPrefab == null)
            return;

        informationObjectInstance = Instantiate(informationObjectPrefab, infoView.transform);
        if (informationObjectInstance == null)
            return;
        informationObjectInstance.transform.Translate(informationPosition);
    }

    public void Hide()
    {
        if (informationObjectInstance == null)
            return;

        Destroy(informationObjectInstance);
        informationObjectInstance = null;
    }
}