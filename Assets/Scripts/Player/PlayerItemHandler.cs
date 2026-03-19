using UnityEngine;
using InputUtil;
public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private float itemDropDistance = 1.5f;
    [SerializeField] private float toggleHandyActionCooldownS;
    [SerializeField] private float attackActionCooldownS;
    [SerializeField] private float throwActionCooldownS;
    [SerializeField] private AnimationParamterInfo toggleHandyAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmAttackAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmThrowAnimationParamter;
    private InputHandlerCooldown attackAction, throwAction;
    private InputHandlerCooldown toggleHandyAction;
    private InventoryItem equippedItem;
    private void Start()
    {
        toggleHandyAction = new("ToggleHandy", toggleHandyActionCooldownS);
        attackAction = new("AttackItem",attackActionCooldownS);
        throwAction = new("ThrowItem",throwActionCooldownS);

        GlobalDataStore.GetStateManager().playerState.playerItemHandler = this;
    }
    private void Update()
    {
        if (toggleHandyAction.InteractWithCooldown())
            toggleHandyAnimationParamter.ValueBool = !toggleHandyAnimationParamter.ValueBool;
        
        if(attackAction.InteractWithCooldown())
            leftArmAttackAnimationParamter.SetTrigger();

        if(throwAction.InteractWithCooldown())
            leftArmThrowAnimationParamter.SetTrigger();

    }
    private void PlayerItemActionResetTrigger()
    {
        leftArmAttackAnimationParamter.ResetTrigger();
        leftArmThrowAnimationParamter.ResetTrigger();
    }
    public void PlayerItemAttack()
    {
        PlayerItemActionResetTrigger();
        Debug.Log("attack");
    }
    public void PlayerItemThrow()
    {
        PlayerItemActionResetTrigger();
        Debug.Log("throw");
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
        if (equippedItem != null)
            UnEquipCurrentItem();

        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
        if (!itemData.HasHeldItemData)
            return false;

        leftArmAnimationParamter.ValueInt = itemData.heldItemData.GeldHeldItemAniamtionIdHash();
        equippedItem = inventoryItem;
        Debug.Log("Equipped Item" + inventoryItem.GetInternalName());
        return true;
    }
    public void UnEquipCurrentItem()
    {
        if (equippedItem == null)
            return;

        Debug.Log("UnEquipped Item" + equippedItem.GetInternalName());
        leftArmAnimationParamter.ValueInt = 0;
        equippedItem = null;
    }
}