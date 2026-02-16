using UnityEngine;
public class ItemEntity : MonoBehaviour
{
    [SerializeField] private string itemEntityId = "";
    [SerializeField] private ItemData itemData;

    private void DestroyItemEntity()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    private void Awake()
    {
        if (itemEntityId == "")
            return;

        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, itemEntityId))
            DestroyItemEntity();
    }
    public void SetItemEntityId(string itemEntityId) { this.itemEntityId = itemEntityId; }
    public void PickupItem()
    {
        if (!GlobalDataStore.GetInventory().PickupItem(itemData.internalName, itemEntityId))
            return;

        DestroyItemEntity();
    }
    public static bool TryDropItem(Camera playerCamera, ItemData itemData, string itemEntityId, float distance)
    {
        Vector3 prefabCreatePosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 prefabSize = itemData.storeableSpawnPrefab.GetComponent<BoxCollider>().size;

        bool isSpaceEmpty = !Physics.CheckBox(prefabCreatePosition, prefabSize / 2);
        if (isSpaceEmpty)
        {
            GameObject ItemEntityObject = Instantiate(itemData.storeableSpawnPrefab, prefabCreatePosition, new Quaternion());
            ItemEntityObject.GetComponent<ItemEntity>().SetItemEntityId(itemEntityId);
        }
        return isSpaceEmpty;
    }
}
