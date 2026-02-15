using UnityEngine;

public class ItemEntity : MonoBehaviour
{
    [SerializeField] private string itemEntityId = "";
    [SerializeField] private ItemData itemData;

    private void DestroyItemEntity()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    private void Awake()
    {
        if(itemEntityId == "")
            return;
        
        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, itemEntityId))
            DestroyItemEntity();
    }
    public void SetItemEntityId(string itemEntityId){this.itemEntityId = itemEntityId;}
    public void PickupItem()
    {
        if (!GlobalDataStore.GetInventory().PickupItem(itemData.internalName, itemEntityId))
            return;

        DestroyItemEntity();
    }
    public static bool TryDropItem(Camera playerCamera,ItemData itemData,string itemEntityId,float distance)
    {
        Vector3 prefabCreatePosition = playerCamera.transform.position + playerCamera.transform.forward * distance ;
        Vector3 prefabSize = playerCamera.transform.TransformDirection( itemData.storeableSpawnPrefab.GetComponent<BoxCollider>().size);
        Vector3 prefabCreatePositionCenter = prefabCreatePosition;
        prefabCreatePositionCenter.y += prefabSize.y / 2;

        if(playerCamera.transform.forward.x <= 0)
            prefabCreatePositionCenter.x -= prefabSize.x / 2;
        else 
            prefabCreatePositionCenter.x += prefabSize.x / 2;
            
       if(playerCamera.transform.forward.z <= 0)
            prefabCreatePositionCenter.z -= prefabSize.z / 2;
        else 
            prefabCreatePositionCenter.z += prefabSize.z / 2;


        // Collision check inacuarrate for spawning Items
        bool state = !Physics.BoxCast(prefabCreatePositionCenter,prefabSize / 2,playerCamera.transform.forward);
        Debug.Log(state);

        if (state)
        {
            GameObject ItemEntityObject = Instantiate(itemData.storeableSpawnPrefab,prefabCreatePosition,new Quaternion());
            ItemEntityObject.GetComponent<ItemEntity>().SetItemEntityId(itemEntityId);
        }
        return state;
    }
}
