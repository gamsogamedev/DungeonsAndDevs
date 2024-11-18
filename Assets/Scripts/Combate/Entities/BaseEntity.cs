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

    // ------ STATS
    private int Health => EntityInfo.GetMaxHealth();
    private int currentHealth;
    public void DoDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{this.gameObject.name} taken {amount} damage\nCurrent Health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // Call death;
        }
        
        // Update Visuals
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
