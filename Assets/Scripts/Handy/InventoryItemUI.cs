using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : InventoryItemUIBase
{
    [SerializeField] private Button dropButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonText;
    new public void SetInventoryItemUI(InventoryItem inventoryItemIn)
    {
        base.SetInventoryItemUI(inventoryItemIn);
        ItemData itemData = inventoryItem.ItemData;

        dropButton.onClick.AddListener(DropButtonOnClick);

        equipButton.enabled = itemData.Equippable;
        equipButton.gameObject.SetActive(itemData.Equippable);
        if (itemData.Equippable)
            equipButton.onClick.AddListener(EquipButtonOnClick);

        if (GlobalDataStore.GetStateManager().playerState.playerItemHandler.EquippedItemInternalName == inventoryItem.InternalName)
        {
            equipButtonText.text = "Unequip";
            equipButton.onClick.AddListener(UnequipButtonOnClick);
        }
    }
    private void EquipButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.playerItemHandler.EquipItem(inventoryItem.InternalName);
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }

    private void UnequipButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.playerItemHandler.UnEquipCurrentItem();
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }
    private void DropButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.playerItemHandler.DropItem(inventoryItem.InternalName);
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }
}