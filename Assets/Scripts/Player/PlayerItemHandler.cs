using UnityEngine;
using InputUtil;
using System;

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
    public string EquippedItemInternalName => equippedItem != null ? equippedItem.InternalName : string.Empty;
    public bool HasItemEquipped => equippedItem != null;
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
    private bool PlayerItemActionStateCheck()
    {
        PlayerItemActionResetTrigger();
        if(!HasItemEquipped)
            return false;
        if(audioSource.isPlaying)
            return false;

        return true;
    }
    public void PlayerItemAttack()
    {
        if(!PlayerItemActionStateCheck())
            return;

        equippedItem.ItemData.heldItemData.attackSoundEffect.Play(audioSource);
    }
    public void PlayerItemThrow()
    {
        if(!PlayerItemActionStateCheck())
            return;

        if (!ThrowItem(equippedItem.InternalName))
            return;

        UpdateEquippedItem();
    }
    public void PlayerItemDrop()
    {
        if(!PlayerItemActionStateCheck())
            return;

        if (!DropItem(equippedItem.InternalName))
            return;
        
        UpdateEquippedItem();
    }
    private void UpdateEquippedItem()
    {
        if(!HasItemEquipped)
            UnEquipCurrentItem();
    }
    private bool DropItemInternal(string internalName,bool throwItem)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        float velocity = 0f;
        if(throwItem)
            velocity = itemData.heldItemData.throwVelocity;

        if(!itemData.Storeable)
            return false;
        if(!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName,out InventoryItem inventoryItem))
            return false;
        
        if(HasItemEquipped)
            if(inventoryItem.InternalName == equippedItem.InternalName && equippedItem.ItemCount == 1)
                UnEquipCurrentItem();
        
        string itemEntityIdNewPrefab = inventoryItem.GetLastItemEntityId();
        return ItemEntity.TryDropItem(playerRef.playerCamera, itemData, itemEntityIdNewPrefab, itemDropDistance,velocity);
    }
    private bool ThrowItem(string internalName)
    {
        return DropItemInternal(internalName,true);
    }
    public bool DropItem(string internalName)
    {
        return DropItemInternal(internalName,false);
    }
    public bool EquipItem(string internalName)
    {
        if (!GlobalDataStore.GetInventory().GetStoreableInventoryItem(internalName, out InventoryItem inventoryItem))
            return false;

        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
        if (!itemData.Equippable)
            return false;

        if (HasItemEquipped)
            UnEquipCurrentItem();

        PlayerItemActionResetTrigger();

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
    public void UnEquipCurrentItem()
    {
        if (equippedItem == null)
            return;

        Debug.Log("UnEquipped Item" + equippedItem.InternalName);
        UnequipItemGameObject();
        
        leftArmAnimationParamter.ValueInt = 0;
        equippedItem = null;
    }
}