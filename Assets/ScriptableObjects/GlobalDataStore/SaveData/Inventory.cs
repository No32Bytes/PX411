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
    private static bool HasItemBeenCollectedInInventory(List<InventoryItem> targetInventory, string internalName, string itemEntityId)
    {
        int index = targetInventory.FindIndex((item) => item.InternalName == internalName);
        if (index != -1)
            return targetInventory[index].HasItemEntityId(itemEntityId);
        return false;
    }
    public bool HasItemBeenCollected(string internalName, string itemEntityId)
    {
        bool collected = HasItemBeenCollectedInInventory(collectableInventory, internalName, itemEntityId);
        if (collected)
            return true;

        return HasItemBeenCollectedInInventory(storeableInventory, internalName, itemEntityId);
    }

    public bool PickupItem(string internalName, string itemEntityId)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);

        if (!itemData.Storeable)
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
        itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);


        if (!itemData.Storeable)
        {
            GetInventoryItemFromInventory(collectableInventory, internalName).RemoveItem();
            return true;
        }

        int index = storeableInventory.FindIndex((item) => item.InternalName == internalName);
        if (index == -1)
            return false;

        storeableInventory[index].RemoveItem();
        if (storeableInventory[index].ItemCount == 0)
            storeableInventory.RemoveAt(index);

        return true;
    }
    public bool GetStoreableInventoryItem(string internalName, out InventoryItem inventoryItem)
    {
        inventoryItem = default;
        int index = storeableInventory.FindIndex((item) => item.InternalName == internalName);
        if (index == -1)
            return false;

        inventoryItem = storeableInventory[index];
        return true;
    }
    private static InventoryItem GetInventoryItemFromInventory(List<InventoryItem> targetInventory, string internalName)
    {
        int index = targetInventory.FindIndex((item) => item.InternalName == internalName);
        if (index != -1)
            return targetInventory[index];

        InventoryItem inventoryItem = new(internalName);
        targetInventory.Add(inventoryItem);
        return inventoryItem;
    }
    public List<InventoryItem> GetCollectableInventory() { return collectableInventory; }
    public List<InventoryItem> GetStoreableInventory() { return storeableInventory; }
}