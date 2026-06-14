using UnityEngine;

public class ItemEntity : BaseEntity
{
    [SerializeField] private ItemData itemData;
    private AudioSource audioSource;
    const float damageDelay = 1f;
    float lastDamage;
    protected override void EntityAwake()
    {
        lastDamage = 0.0f;
        audioSource = AudioUtil.CreateSoundEffectAudioSource(gameObject);
        if (GlobalDataStore.GetInventory().HasItemBeenCollected(itemData.internalName, entityId))
            DestroyEntity();
    }
    public override void EntityInteraction()
    {
        if (itemData == null)
            throw new System.Exception("ItemEntity has no itemData associated with it.");

        if (!GlobalDataStore.GetInventory().PickupItem(itemData.internalName, entityId))
            return;

        PlayerItemHandler playerItemHandler = GlobalDataStore.GetStateManager().playerState.playerItemHandler;
        if (!playerItemHandler.HasItemEquipped)
            playerItemHandler.EquipItem(itemData.internalName);

        DestroyEntity();
    }
    public static bool TryDropItem(Camera playerCamera, ItemData itemData, string itemEntityId, float distance, float startVelocity = 0f)
    {
        Vector3 prefabCreatePosition = playerCamera.transform.position + playerCamera.transform.forward * distance;
        Vector3 prefabSize = itemData.storeableItemData.spawnPrefab.GetComponent<Collider>().bounds.size;

        bool isSpaceEmpty = !Physics.CheckBox(prefabCreatePosition, prefabSize / 2);
        if (isSpaceEmpty)
        {
            GlobalDataStore.GetInventory().DropItem(itemData.internalName, out _);
            GameObject ItemEntityObject = Instantiate(itemData.storeableItemData.spawnPrefab, prefabCreatePosition, new Quaternion());
            ItemEntityObject.name = itemEntityId + " - " + itemData.internalName;

            ItemEntity itemEntity = ItemEntityObject.GetComponent<ItemEntity>();
            itemEntity.SetBaseEntityId(itemEntityId);
            itemEntity.GetEntityRigibody().linearVelocity = startVelocity * playerCamera.transform.forward;

        }
        return isSpaceEmpty;
    }

    private bool CanDamage()
    {
        if (lastDamage + damageDelay > Time.time)
            return false;

        if (!gameObject.TryGetComponent(out Rigidbody rigidbody))
        {
            if (rigidbody.linearVelocity.magnitude < itemData.heldItemData.minDamageVelocity)
                return false;
        }
        return true;
    }

    void OnTriggerEnter(Collider collider)
    {
        bool canDamage = CanDamage();


        if (!collider.gameObject.TryGetComponent(out Player player))
        {
            AudioUtil.PlaySoundEffect(itemData.collisionSoundEffect, audioSource);
            if (canDamage)
            {
                player.Damage(itemData.throwDamage);
                lastDamage = Time.time;
            }
            return;
        }

        if (!collider.gameObject.TryGetComponent(out EnemeyEntity enemeyEntity))
        {
            AudioUtil.PlaySoundEffect(itemData.collisionSoundEffect, audioSource);
            if (canDamage)
            {
                enemeyEntity.EntityDamage(itemData.throwDamage);
                lastDamage = Time.time;
            }
            return;
        }
    }
}
