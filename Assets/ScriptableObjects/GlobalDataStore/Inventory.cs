using System;
using System.Collections.Generic;

[Serializable]
public class ItemCollectable
{
    public string internalName = "";
    public int itemCount = 0;
    public List<string> collectedItemEntityIDs = new();
    public ItemCollectable(string internalName)
    {
        this.internalName = internalName;
    }
    public void CollectItem(string itemEntityId)
    {
        itemCount++;
        collectedItemEntityIDs.Add(itemEntityId);
    }
    public bool HasItemEntityID(string itemEntityId)
    {
        return collectedItemEntityIDs.Contains(itemEntityId);
    }
}
[Serializable]
public class ItemStoreable
{
    
}
[Serializable]
public class Inventory
{
    public List<ItemCollectable> collectableInventory = new();
    public bool HasCollectableItemBeenCollected(string internalName,string itemEntityId)
    {
        int index = collectableInventory.FindIndex((collectable) => collectable.internalName == internalName);
        if(index == -1)
            return false;
        
        return collectableInventory[index].HasItemEntityID(itemEntityId);
    }
    public ItemCollectable GetCollectableItem(string internalName)
    {
        int index = collectableInventory.FindIndex((collectable) => collectable.internalName == internalName);
        if (index == -1)
        {
            ItemCollectable itemCollectable = new(internalName);
            collectableInventory.Add(itemCollectable);
            return itemCollectable;
        }
        return collectableInventory[index];
    }
}