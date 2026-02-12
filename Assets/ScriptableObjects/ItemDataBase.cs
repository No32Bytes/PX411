
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase",menuName = "Item/ItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField] private List<ItemData> itemDataBase = new();
    public bool GetItemDataFromInternalName(string internalName,out ItemData itemData)
    {
        itemData = null;
        int index = itemDataBase.FindIndex((item) => item.internalName == internalName);
        if(index == -1) 
            return false;

        itemData = itemDataBase[index];
        return true;
    }
}