using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    [SerializeField] private string itemEntityId;
    [SerializeField] private ItemData itemData;

    private void DestroyItemEntity()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    private void Awake()
    {
        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, itemEntityId))
            DestroyItemEntity();
    }
    
    public void PickupItem()
    {
        if(!GlobalDataStore.GetInventory().PickupItem(itemData.internalName,itemEntityId))
            return;
        
        DestroyItemEntity();
    }

}
