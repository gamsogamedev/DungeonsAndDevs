using System;
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
    [Header("Entity Info")] // ------ Basic Info
    public string entityName;
    public EntityType entityType;

    // ------ Prefab
    protected BaseEntity entityPrefab;
    
    // ------ Converters
    public abstract ScriptableEntity_Playable ToPlayable(); 
    public abstract ScriptableEntity_Hostile ToHostile();
    
    // ------ Instance
    public BaseEntity EntityInstance { get; private set; }
    public virtual void GenerateEntity()
    {
        EntityInstance = Instantiate(entityPrefab);
        
    }
}