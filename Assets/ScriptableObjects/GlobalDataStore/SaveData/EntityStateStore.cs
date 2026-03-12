using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class EntityState
{
    public string entityId = "";
    public bool isEnabled = true;
    public EntityState(string entityId)
    {
        this.entityId = entityId;
    }
    public string entityStateData = "";
}

[Serializable]
public class EntityStateStore
{
    [SerializeField] private List<EntityState> entityStateStore = new();
    private bool FindEntityState(string entityId, out EntityState entityState)
    {
        entityState = null;
        int index = entityStateStore.FindIndex((entityState) => entityState.entityId == entityId);
        if (index == -1)
            return false;

        entityState = entityStateStore[index];
        return true;
    }
    private EntityState GetEntityState(string entityId)
    {
        if (!FindEntityState(entityId, out EntityState entityState))
            entityState = new(entityId);
        return entityState;
    }
    public void SetEntityIdEnabledState(string entityId, bool isEnabled)
    {
        GetEntityState(entityId).isEnabled = isEnabled;
    }
    public bool GetEntityIdEnabledState(string entityId)
    {
        return GetEntityState(entityId).isEnabled;
    }
    public void SetEntityStateData(string entityId, string entityStateData)
    {
        GetEntityState(entityId).entityStateData = entityStateData;
    }
    public string GetEntityStateData(string entityId)
    {
        return GetEntityState(entityId).entityStateData;
    }
}