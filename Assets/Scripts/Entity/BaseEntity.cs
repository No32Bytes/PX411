using UnityEngine;
public abstract class BaseEntity : MonoBehaviour
{
    [SerializeField] protected string entityId;
    [SerializeField] protected bool usePhysics = true;
    private void Awake()
    {
        if (string.IsNullOrEmpty(entityId))
            return;

        if (!GlobalDataStore.GetSaveData().entityStateStore.GetEntityIdEnabledState(entityId))
        {
            DestroyEntity();
            return;
        }

        SetPhysicsState(usePhysics);

        EntityAwake();
    }
    private Rigidbody GetEntityRigibody()
    {
        if (!gameObject.TryGetComponent(out Rigidbody entityRigibody))
            entityRigibody = gameObject.AddComponent<Rigidbody>();
        return entityRigibody;
    }
    public void SetPhysicsState(bool enabled)
    {
        usePhysics = enabled;

        Rigidbody rigidbody = GetEntityRigibody();
        rigidbody.useGravity = usePhysics;
        rigidbody.isKinematic = !usePhysics;
    }
    protected virtual void EntityAwake() { }
    public virtual void EntityInteraction(){}
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
