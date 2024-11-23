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
    public Sprite entityVisuals;
    
    // ------ Stats
    [Header("Stats")]
    [SerializeField] protected int maxHealth;
    public int GetMaxHealth() => maxHealth;
    [SerializeField] protected int stamina;
    public int GetStamina() => stamina;
    [SerializeField] protected int speed;
    public int GetSpeed() => speed;
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