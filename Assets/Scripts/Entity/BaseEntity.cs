using UnityEngine;
public abstract class BaseEntity : MonoBehaviour
{
    [SerializeField] protected string entityId;
    [SerializeField] protected bool useGravity = true;
    private void Awake()
    {
        if (string.IsNullOrEmpty(entityId))
            return;

        if (!GlobalDataStore.GetSaveData().entityStateStore.GetEntityIdEnabledState(entityId))
        {
            DestroyEntity();
            return;
        }

        AwakeGravity();

        EntityAwake();
    }
    private void AwakeGravity()
    {
        Rigidbody entityRigibody;
        if (!useGravity)
        {
            if (gameObject.TryGetComponent(out entityRigibody))
                DestroyImmediate(entityRigibody);
            return;
        }

        if (!gameObject.TryGetComponent(out entityRigibody))
            entityRigibody = gameObject.AddComponent<Rigidbody>();

        entityRigibody.useGravity = useGravity;
    }
    protected virtual void EntityAwake() { }
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
