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
    private InventoryItem inventoryItem;
    public void SetInventoryItemUI(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
        ItemData itemData = inventoryItem.GetItemData();

        itemIcon.overrideSprite = itemData.icon;

        itemName.text = itemData.displayName;

        if (inventoryItem.GetItemCount() == 1)
            itemCount.text = "";
        else
            itemCount.text = inventoryItem.GetItemCount() + "x";

        itemDescription.text = itemData.description;

        dropButton.onClick.AddListener(DropButtonOnClick);


        equipButton.enabled = itemData.HasHeldItemData;
        equipButton.gameObject.SetActive(itemData.HasHeldItemData);
        if (itemData.HasHeldItemData)
            equipButton.onClick.AddListener(EquipButtonOnClick);
    }
    private void EquipButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.playerItemHandler.EquipItem(inventoryItem.GetInternalName());
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();
    }
    private void DropButtonOnClick()
    {
        GlobalDataStore.GetStateManager().playerState.playerItemHandler.DropItem(inventoryItem.GetInternalName());
        GlobalDataStore.GetStateManager().playerState.player.DisableHandyScreenUI();

    }
}