using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private string internalName;
    [SerializeField] private int itemCount = 0;
    [SerializeField] private List<string> itemEntityIdsCollected = new();
    public int ItemCount => itemCount;
    public string InternalName => internalName;
    public ItemData ItemData => GlobalDataStore.GetItemDataBase().GetItemDataFromInternalName(internalName);
    public InventoryItem(string internalName)
    {
        this.internalName = internalName;
    }

    public bool HasItemEntityId(string itemEntityId)
    {
        return itemEntityIdsCollected.Contains(itemEntityId);
    }

    public bool PickupItem(string itemEntityId)
    {
        if (HasItemEntityId(itemEntityId)) return true;
        itemCount++;
        itemEntityIdsCollected.Add(itemEntityId);
        return true;
    }

    public bool RemoveItem()
    {
        if (itemCount == 0) return false;
        if (itemEntityIdsCollected.Count == 0) return false;

        itemCount--;
        itemEntityIdsCollected.RemoveAt(itemEntityIdsCollected.Count - 1);
        return true;
    }
    public bool RemoveItemForever()
    {
        string id = GetLastItemEntityId();
        if (string.IsNullOrEmpty(id))
            return false;
        GlobalDataStore.GetSaveData().entityStateStore.SetEntityIdEnabledState(id, false);
        RemoveItem();
        return true;
    }
    public string GetLastItemEntityId()
    {
        if (itemEntityIdsCollected.Count == 0) return "";
        return itemEntityIdsCollected[^1];
    }
}