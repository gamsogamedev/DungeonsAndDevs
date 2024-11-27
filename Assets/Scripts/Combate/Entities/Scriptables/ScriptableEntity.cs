using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum EntityType
{
    All,
    Playable,
    Hostile
} 

public abstract class ScriptableEntity : ScriptableObject
{
    [Header("Entity Info")] // ------ Basic Info
    public string entityName;
    public EntityType entityType;

    // ------ Prefab
    [SerializeField] protected BaseEntity entityPrefab;
    public Sprite entityVisuals;
    
    // ------ Stats
    [Header("Stats")]
    [SerializeField] protected int maxHealth;
    public int GetMaxHealth() => maxHealth;
    [SerializeField] protected int initiative;
    public int GetInitiative() => initiative;
    [SerializeField] protected int speed;
    public int GetSpeed() => speed;
    // ------ Converters
    public abstract ScriptableEntity_Playable ToPlayable(); 
    public abstract ScriptableEntity_Hostile ToHostile();
    
    // ------ Instance
    public abstract BaseEntity GenerateEntity();
}