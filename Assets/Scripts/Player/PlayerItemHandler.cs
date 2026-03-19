using UnityEngine;
using InputUtil;
using Unity.VisualScripting;
public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private float itemDropDistance = 1.5f;
    [SerializeField] private float toggleHandyActionCooldownS;
    [SerializeField] private AnimationParamterInfo toggleHandyAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmAnimationParamter;
    private InputHandlerCooldown toggleHandyAction;
    private InventoryItem equippedItem;
    private void Start()
    {
        toggleHandyAction = new("ToggleHandy", toggleHandyActionCooldownS);
        GlobalDataStore.GetStateManager().playerState.playerItemHandler = this;
    }
    private void Update()
    {
        if (toggleHandyAction.InteractWithCooldown())
            toggleHandyAnimationParamter.ValueBool = !toggleHandyAnimationParamter.ValueBool;
    }

    public bool DropItem(string internalName)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        if (!itemData.storeable || itemData.storeableSpawnPrefab == null)
            return false;
        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        return ItemEntity.TryDropItem(playerRef.playerCamera, itemData, itemEntityIdNewPrefab, itemDropDistance);
    }
    public bool EquipItem(string internalName)
    {
        if(equippedItem != null)
            UnEquipCurrentItem();

        if(!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName,out InventoryItem inventoryItem))
            return false;

        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
        if(!itemData.HasHeldItemData)
            return false;

        leftArmAnimationParamter.ValueInt = itemData.heldItemData.GeldHeldItemAniamtionIdHash();
        equippedItem = inventoryItem;
        return true;
    }
    public void UnEquipCurrentItem()
    {
        if(equippedItem == null)
            return;
        
        leftArmAnimationParamter.ValueInt = 0;
        equippedItem = null;
    }
}