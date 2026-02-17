using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [SerializeField] protected string entityId;
    protected bool entityState = true;

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

    public void SetBaseEntityId(string entityId)
    {
        this.entityId = entityId;
        Awake();
    }
    public string GetBaseEntityId() { return entityId; }
    protected void DestroyEntity()
    {
        entityState = false;
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    public abstract void EntityInteraction();
}
