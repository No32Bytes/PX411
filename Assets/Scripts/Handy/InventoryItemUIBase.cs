using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryItemUIBase : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemCount;
    [SerializeField] private TMP_Text itemDescription;
    protected InventoryItem inventoryItem;
    public void SetInventoryItemUI(InventoryItem inventoryItemIn)
    {
        inventoryItem = inventoryItemIn;
        ItemData itemData = inventoryItem.ItemData;

        itemIcon.overrideSprite = itemData.icon;

        itemName.text = itemData.displayName;

        if (inventoryItem.ItemCount == 1)
            itemCount.text = "";
        else
            itemCount.text = inventoryItem.ItemCount + "x";

        itemDescription.text = itemData.description;
    }
}