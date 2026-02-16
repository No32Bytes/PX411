using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    [SerializeField] protected string entityId;

    public void SetBaseEntityId(string entityId) { this.entityId = entityId; }
    public string GetBaseEntityId() { return entityId; }
    protected void DestroyEntity()
    {
        gameObject.hideFlags = HideFlags.DontSave;
        DestroyImmediate(gameObject);
    }
    public abstract void EntityInteraction();
}
