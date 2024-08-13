using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Playable,
    Hostile
} 

public abstract class ScriptableEntity : ScriptableObject
{
    public BaseEntity entityPrefab;

    public string entityName;
    public EntityType entityType;

    public abstract ScriptableEntity_Playable ToPlayable(); 
    //public abstract HostileEntity ToHostile();
    
    public virtual void GenerateEntity()
    {
        var entity = Instantiate(entityPrefab);
        entity.InitializeEntity(this);
    }
}