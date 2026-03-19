using UnityEngine;

[CreateAssetMenu(fileName = "HeldItemData", menuName = "Item/HeldItemData")]
public class HeldItemData : ScriptableObject
{   
    public GameObject heldItemPrefab;
    public string heldItemAnimationId;
    public int GeldHeldItemAniamtionIdHash()
    {
        if(string.IsNullOrEmpty(heldItemAnimationId))
            return 0;
        
        int hash = 0;
        int v = 0;
        foreach(char c in heldItemAnimationId)
        {
            v += c.GetHashCode();
            hash += c + v % c;
            hash <<= 1;
        }
        return hash * heldItemAnimationId.Length;
    }
}