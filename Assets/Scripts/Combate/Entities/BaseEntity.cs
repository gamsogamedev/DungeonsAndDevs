using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseEntity : MonoBehaviour
{
    // ------ BASIC INFO
    public ScriptableEntity EntityInfo; // { get; protected set; }
        
    // ------ GRID STATE INFO
    [HideInInspector] public Cell currentCell;

    internal virtual void Start()
    {
        currentHealth = Health;
    }

    public virtual void InitializeEntity(ScriptableEntity entity)
    {
        EntityInfo = entity;
        
        transform.Find("Visuals").GetComponent<SpriteRenderer>().sprite = EntityInfo.entityVisuals;

        GetComponentInChildren<HealthHUD>().Init(this);
    }

    // ------ STATS
    private int Health => EntityInfo.GetMaxHealth();
    private int currentHealth;

    public readonly UnityEvent<float> HealthUpdated = new();
    
    public void DoDamage(int amount)
    {
        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            CombatManager.OnEntityDeath?.Invoke(this);
        }

        HealthUpdated?.Invoke((float) currentHealth / Health);
    }
    
    private int MovementRange => EntityInfo.GetSpeed();
    [HideInInspector] public int currentMovement;
    
    // ------- EVENTS
    public readonly UnityEvent EntitySelected = new();
    public readonly UnityEvent OnEntityMoved = new();
    
    // ------- MOVEMENT
    public abstract void MoveTowards(Cell cellToMove, bool blink = false);
    public void ResetMovement() => currentMovement = MovementRange;
}

public interface IEntity
{
    void InitializeEntity(ScriptableEntity entityInfo);
}
