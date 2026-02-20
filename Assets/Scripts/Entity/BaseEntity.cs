using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [SerializeField] protected string entityId;
    private void Awake()
    {
        if (string.IsNullOrEmpty(entityId))
            return;

        if (!GlobalDataStore.GetSaveData().entityStateStore.GetEntityIdEnabledState(entityId))
        {
            DestroyEntity();
            return;
        }
        EntityAwake();
    }
    protected abstract void EntityAwake();
    public abstract void EntityInteraction();
    public void SetBaseEntityId(string entityId)
    {
        this.entityId = entityId;
        Awake();
    }
    public string GetBaseEntityId() { return entityId; }
    protected void DestroyEntity()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
}
