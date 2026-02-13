using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private string internalName;
    [SerializeField] private int itemCount = 0;
    [SerializeField] private List<string> itemEntityIdsCollected = new();
    public InventoryItem(string internalName)
    {
        this.internalName = internalName;
    }

    public string GetInternalName() { return internalName; }

    public int GetItemCount() { return itemCount; }

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

        itemEntityIdsCollected.RemoveAt(itemEntityIdsCollected.Count - 1);
        return true;
    }
}