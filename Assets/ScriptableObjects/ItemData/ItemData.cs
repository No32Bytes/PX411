using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public string internalName;
    public Sprite icon;
    [TextArea] public string description;

    [Serializable] public struct StoreableItemData
    {
        public bool storeable;
        public GameObject spawnPrefab;
    }
    public StoreableItemData storeableItemData;
    public bool Storeable => storeableItemData.storeable;
    
    [Serializable] public struct HeldItemData
    {
        public bool equippable;
        public GameObject heldItemPrefab;
    }
    public HeldItemData heldItemData;
    public bool Equippable => heldItemData.equippable;
    public int InternalNameIntHash
    {
        get
        {
            if (string.IsNullOrEmpty(internalName))
                return 0;

            int hash = 0;
            int v = 0;
            foreach (char c in internalName)
            {
                v += c.GetHashCode();
                hash += c + v % c;
                hash <<= 1;
            }
            return hash;
        }
    }

}
