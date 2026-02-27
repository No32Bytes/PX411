using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private InventoryItem inventoryItem;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemDescription;
    public void SetInventoryItemUI(InventoryItem inventoryItem)
    {
        ItemData itemData = inventoryItem.GetItemData();
        
        itemIcon.overrideSprite = itemData.icon;
    
        itemName.text = itemData.displayName;
        
        if(inventoryItem.GetItemCount() == 1)
            itemCount.text = "";
        else 
            itemCount.text = inventoryItem.GetItemCount() +"x";

        itemDescription.text = itemData.description;
    }
}