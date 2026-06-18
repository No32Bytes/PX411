using UnityEngine;

public class LockDoorEntity : DoorEntity
{

    [System.Serializable]
    public struct Requirement
    {
        public ItemData itemData;
        public int optionalcount;
        public string optionalTargetItemEntityID;
    };
    [SerializeField] Requirement[] requirements;
    bool completedRequirements;
    private void Awake()
    {
        entityId = "door";
        usePhysics = false;
        completedRequirements = false;
    }

    private void AllRequirementsCompleted()
    {
        if (completedRequirements)
            return;

        foreach (Requirement req in requirements)
        {
            if (!GlobalDataStore.GetInventory().GetInventoryItem(req.itemData.internalName, out InventoryItem item))
                return;

            if (!string.IsNullOrEmpty(req.optionalTargetItemEntityID))
            {
                if (!item.HasItemEntityId(req.optionalTargetItemEntityID))
                    return;
                continue;
            }

            if (item.ItemCount < req.optionalcount)
                return;

        }

        completedRequirements = true;
    }

    public override void EntityInteraction()
    {
        AllRequirementsCompleted();
        if (completedRequirements)
            ToggleDoorState();
    }
}