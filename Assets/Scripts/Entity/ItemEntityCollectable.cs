using UnityEngine;

public class ItemEntityCollectable : MonoBehaviour
{
    [SerializeField] private string itemEntityId;
    [SerializeField] private ItemData itemData;
    
    private void DestroyItem()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    private void Awake()
    {
        if(GlobalDataStore.GetSaveData().inventory.HasCollectableItemBeenCollected(itemData.internalName, itemEntityId))
            DestroyItem();
    }
    public void CollectItem()
    {
        GlobalDataStore.GetSaveData().inventory.GetCollectableItem(itemData.internalName).CollectItem(itemEntityId);
        DestroyItem();
    }
}