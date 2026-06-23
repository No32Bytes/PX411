using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryConfig
{
    public static readonly int MaxInventorySize = 20;
}

[Serializable]
public class Inventory
{
    [SerializeField] private List<InventoryItem> collectableInventory = new();
    [SerializeField] private List<InventoryItem> storeableInventory = new();
    public List<string> flag = new();
    public void AddFlag(string flagStr)
    {
        if (!flag.Contains(flagStr))
            flag.Add(flagStr);
    }
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

        return true;
    }
    public void RemoveItemFromInventory(string internalName)
    {
        ItemData itemData = GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
        if (!itemData.Storeable)
            return;

        int index = storeableInventory.FindIndex((item) => item.InternalName == internalName);
        if (index == -1)
            return;

        if (storeableInventory[index].ItemCount == 0)
        {
            storeableInventory.RemoveAt(index);
            return;
        }
        storeableInventory[index].RemoveItem();
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
    public bool GetCollectableInventoryItem(string internalName, out InventoryItem inventoryItem)
    {
        inventoryItem = default;
        int index = collectableInventory.FindIndex((item) => item.InternalName == internalName);
        if (index == -1)
            return false;

        inventoryItem = collectableInventory[index];
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

    public bool GetInventoryItem(string internalName, out InventoryItem inventoryItem)
    {

        if (GetCollectableInventoryItem(internalName, out inventoryItem))
            return true;

        return GetStoreableInventoryItem(internalName, out inventoryItem);
    }
}