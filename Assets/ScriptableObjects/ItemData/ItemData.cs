using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public string internalName;
    public bool storeable;
    public GameObject storeableSpawnPrefab;
    public Sprite icon;
    public HeldItemData optionalheldItemData = null;
    [TextArea] public string description;
}
