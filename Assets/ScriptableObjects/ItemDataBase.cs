
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "Item/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField] private List<ItemData> itemDataBase = new();
    public bool GetItemDataFromInternalName(string internalName, out ItemData itemData)
    {
        itemData = null;
        int index = itemDataBase.FindIndex((item) => item.internalName == internalName);
        if (index == -1)
            throw new System.Exception("internalName of an Item must point to an itemData object in the ItemDataBase. It doesn't :(");

        itemData = itemDataBase[index];
        return true;
    }
}