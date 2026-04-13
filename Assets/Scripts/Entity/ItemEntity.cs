using UnityEngine;

public class ItemEntity : BaseEntity
{
    [SerializeField] private ItemData itemData;

    protected override void EntityAwake()
    {
        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, entityId))
            DestroyEntity();
    }
    public override void EntityInteraction()
    {
        if (!GlobalDataStore.GetInventory().PickupItem(itemData.internalName, entityId))
            return;

        PlayerItemHandler playerItemHandler = GlobalDataStore.GetStateManager().playerState.playerItemHandler;
        if(!playerItemHandler.HasItemEquipped)
            playerItemHandler.EquipItem(itemData.internalName);

        DestroyEntity();
    }
    public static bool TryDropItem(Camera playerCamera, ItemData itemData, string itemEntityId, float distance, float startVelocity = 0f)
    {
        Vector3 prefabCreatePosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 prefabSize = itemData.storeableItemData.spawnPrefab.GetComponent<BoxCollider>().size;

        bool isSpaceEmpty = !Physics.CheckBox(prefabCreatePosition, prefabSize / 2);
        if (isSpaceEmpty)
        {
            GlobalDataStore.GetInventory().DropItem(itemData.internalName, out _);
            GameObject ItemEntityObject = Instantiate(itemData.storeableItemData.spawnPrefab, prefabCreatePosition, new Quaternion());
            ItemEntityObject.name = itemEntityId + " - " + itemData.internalName;

            ItemEntity itemEntity = ItemEntityObject.GetComponent<ItemEntity>();
            itemEntity.SetBaseEntityId(itemEntityId);
            itemEntity.GetEntityRigibody().linearVelocity = startVelocity * playerCamera.transform.forward;

        }
        return isSpaceEmpty;
    }
}
