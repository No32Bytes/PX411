using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonText;
    private InventoryItem inventoryItem;
    public void SetInventoryItemUI(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        ItemData itemData = inventoryItem.ItemData;

        itemIcon.overrideSprite = itemData.icon;

        itemName.text = itemData.displayName;

        if (inventoryItem.ItemCount == 1)
            itemCount.text = "";
        else
            itemCount.text = inventoryItem.ItemCount + "x";

        itemDescription.text = itemData.description;

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