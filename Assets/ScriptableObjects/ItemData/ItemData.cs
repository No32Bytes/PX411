using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public string internalName;
    public Sprite icon;
    [TextArea] public string description;
    public SimpleSoundEffect collisionSoundEffect;

    [Serializable]
    public struct StoreableItemData
    {
        public bool storeable;
        public GameObject spawnPrefab;
    }
    public StoreableItemData storeableItemData;
    public bool Storeable
    {
        get
        {
            return storeableItemData.storeable && storeableItemData.spawnPrefab != null;
        }
    }

    [Serializable]
    public struct HeldItemData
    {
        public bool equippable;
        public GameObject heldItemPrefab;
        public float throwVelocity;
        public SimpleSoundEffect attackSoundEffect;
    }
    public HeldItemData heldItemData;
    public bool Equippable
    {
        get
        {
            return heldItemData.equippable && heldItemData.heldItemPrefab != null;
        }
    }
    public int InternalNameIntHash => HashUtil.FNV1.ComputeHash_24Bytes(internalName);

    public float weaponDamage = 0.0f;
}
