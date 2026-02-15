using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryConfig
{
    public static readonly int MaxInventorySize = 10;
}

[Serializable]
public class Inventory
{
    [SerializeField] private List<InventoryItem> collectableInventory = new();
    [SerializeField] private List<InventoryItem> storeableInventory = new();
    public bool HasItemBeenCollected(string internalName, string itemEntityId)
    {
        int index = collectableInventory.FindIndex((collectable) => collectable.GetInternalName() == internalName);
        if (index != -1)
            return collectableInventory[index].HasItemEntityId(itemEntityId);

        index = storeableInventory.FindIndex((storeable) => storeable.GetInternalName() == internalName);
        if (index != -1)
            return collectableInventory[index].HasItemEntityId(itemEntityId);

        return false;
    }

    public bool PickupItem(string internalName, string itemEntityId)
    {
        if (!GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName, out ItemData itemData))
            return false;

        if (!itemData.storeable)
        {
            GetInventoryItemFromInventory(collectableInventory, internalName).PickupItem(itemEntityId);
            return true;
        }

        if (storeableInventory.Count == InventoryConfig.MaxInventorySize) return false;
        GetInventoryItemFromInventory(storeableInventory, internalName).PickupItem(itemEntityId);
        return true;
    }

    public bool DropItem(string internalName, out ItemData itemData)
    {
        if (!GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName, out itemData))
            return false;

        if (!itemData.storeable)
        {
            GetInventoryItemFromInventory(collectableInventory, internalName).RemoveItem();
            return true;
        }

        int index = storeableInventory.FindIndex((item) => item.GetInternalName() == internalName);
        if (index == -1)
            return false;

        storeableInventory[index].RemoveItem();
        if (storeableInventory[index].GetItemCount() == 0)
            storeableInventory.RemoveAt(index);

        return true;
    }
    public bool GetStoreableInventoryItem(string internalName,out InventoryItem inventoryItem)
    {
        inventoryItem = default;
        int index = storeableInventory.FindIndex((item) => item.GetInternalName() == internalName);
        if(index == -1)
            return false;
        
        inventoryItem = storeableInventory[index];
        return true;
    }
    private InventoryItem GetInventoryItemFromInventory(List<InventoryItem> targetInventory, string internalName)
    {
        int index = targetInventory.FindIndex((item) => item.GetInternalName() == internalName);
        if (index != -1)
            return collectableInventory[index];

        InventoryItem inventoryItem = new(internalName);
        targetInventory.Add(inventoryItem);
        return inventoryItem;
    }
    public List<InventoryItem> GetCollectableInventory() { return collectableInventory; }
    public List<InventoryItem> GetStoreableInventory() { return storeableInventory; }
}