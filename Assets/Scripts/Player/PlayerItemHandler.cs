using UnityEngine;
using InputUtil;

public class PlayerItemHandler : MonoBehaviour
{
    [SerializeField] private PlayerReferences playerRef;
    [SerializeField] private float itemDropDistance = 1.5f;
    [SerializeField] private float toggleHandyActionCooldownS;
    [SerializeField] private float attackActionCooldownS;
    [SerializeField] private float throwActionCooldownS;
    [SerializeField] private float dropActionCooldownS;
    [Header("Animation Paramters")]
    [SerializeField] private AnimationParamterInfo toggleHandyAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmAttackAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmThrowAnimationParamter;
    [SerializeField] private AnimationParamterInfo leftArmDropAnimatiomParamter;
    private InputHandlerCooldown attackAction, throwAction, dropAction;
    private InputHandlerCooldown toggleHandyAction;
    private InventoryItem equippedItem;
    private GameObject equippedItemGameObject;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start()
    {
        toggleHandyAction = new("ToggleHandy", toggleHandyActionCooldownS);
        attackAction = new("AttackItem", attackActionCooldownS);
        throwAction = new("ThrowItem", throwActionCooldownS);
        dropAction = new("DropItem", dropActionCooldownS);

        GlobalDataStore.GetStateManager().playerState.playerItemHandler = this;
    }
    private void Update()
    {
        if (toggleHandyAction.InteractWithCooldown())
            toggleHandyAnimationParamter.ValueBool = !toggleHandyAnimationParamter.ValueBool;

        if (attackAction.InteractWithCooldown())
            leftArmAttackAnimationParamter.SetTrigger();

        if (throwAction.InteractWithCooldown())
            leftArmThrowAnimationParamter.SetTrigger();

        if (dropAction.InteractWithCooldown())
            leftArmDropAnimatiomParamter.SetTrigger();

    }
    private void PlayerItemActionResetTrigger()
    {
        leftArmAttackAnimationParamter.ResetTrigger();
        leftArmThrowAnimationParamter.ResetTrigger();
        leftArmDropAnimatiomParamter.ResetTrigger();
    }
    public void PlayerItemAttack()
    {
        PlayerItemActionResetTrigger();
        equippedItem.ItemData.heldItemData.attackSoundEffect.Play(audioSource);
    }
    public void PlayerItemThrow()
    {
        PlayerItemActionResetTrigger();
        if (equippedItem == null)
            return;

        if (!ThrowItem(equippedItem))
            return;

        if (equippedItem.ItemCount == 0)
            UnEquipCurrentItem();
    }
    public void PlayerItemDrop()
    {
        PlayerItemActionResetTrigger();
        if (equippedItem == null)
            return;

        if (!DropItem(equippedItem.InternalName))
            return;

        if (equippedItem.ItemCount == 0)
            UnEquipCurrentItem();
    }
    private bool ThrowItem(InventoryItem inventoryItem)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(inventoryItem.InternalName);

        if (!itemData.Storeable)
            return false;

        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        return ItemEntity.TryDropItem(playerRef.playerCamera, itemData, itemEntityIdNewPrefab, itemDropDistance, itemData.heldItemData.throwVelocity);
    }
    public bool DropItem(string internalName)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        if (!itemData.Storeable)
            return false;
        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        return ItemEntity.TryDropItem(playerRef.playerCamera, itemData, itemEntityIdNewPrefab, itemDropDistance);
    }
    public bool EquipItem(string internalName)
    {
        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
        if (!itemData.Equippable)
            return false;

        if (equippedItem != null)
            UnEquipCurrentItem();

        leftArmAnimationParamter.ValueInt = itemData.InternalNameIntHash;
        equippedItem = inventoryItem;

        EquipItemGameObject();
        Debug.Log("Equipped Item" + inventoryItem.InternalName);

        return true;
    }
    private void EquipItemGameObject()
    {
        equippedItemGameObject = Instantiate(equippedItem.ItemData.heldItemData.heldItemPrefab,playerRef.leftPlayerArmItemAnchor.transform);
    }
    private void UnequipItemGameObject()
    {
        Destroy(equippedItemGameObject);
        equippedItemGameObject = null;
    }
    private void UnEquipCurrentItem()
    {
        if (equippedItem == null)
            return;

        Debug.Log("UnEquipped Item" + equippedItem.InternalName);
        UnequipItemGameObject();
        
        leftArmAnimationParamter.ValueInt = 0;
        equippedItem = null;
    }
}