using UnityEngine;

public class ItemEntity : BaseEntity
{
    [SerializeField] private ItemData itemData;

    private void Awake()
    {
        if(string.IsNullOrEmpty(entityId))
            return;
        
        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, entityId))
            DestroyEntity();
    }
    public override void EntityInteraction()
    {
        if (!GlobalDataStore.GetInventory().PickupItem(itemData.internalName, entityId))
            return;

        DestroyEntity();
    }

    public static bool TryDropItem(Camera playerCamera, ItemData itemData, string itemEntityId, float distance)
    {
        Vector3 prefabCreatePosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 prefabSize = itemData.storeableSpawnPrefab.GetComponent<BoxCollider>().size;

        bool isSpaceEmpty = !Physics.CheckBox(prefabCreatePosition, prefabSize / 2);
        if (isSpaceEmpty)
        {
            GameObject ItemEntityObject = Instantiate(itemData.storeableSpawnPrefab, prefabCreatePosition, new Quaternion());
            ItemEntityObject.GetComponent<ItemEntity>().SetBaseEntityId(itemEntityId);
        }
        return isSpaceEmpty;
    }
}
