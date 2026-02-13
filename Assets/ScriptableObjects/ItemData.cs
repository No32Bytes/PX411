using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public string internalName;
    public bool storeable;
    public Sprite icon;
    [TextArea] public string description;
}
