using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject inventoryItemUI;
    void Start()
    {
        List<InventoryItem> inventory = GlobalDataStore.GetInventory().GetCollectableInventory();
        InventoryItemUI inventorySlot = Instantiate(inventoryItemUI,transform).GetComponent<InventoryItemUI>();
        inventorySlot.SetInventoryItemUI(inventory[0]);
    }
}
