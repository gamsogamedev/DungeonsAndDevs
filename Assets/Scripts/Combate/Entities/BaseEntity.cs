using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseEntity : MonoBehaviour
{
    // ------ BASIC INFO
    public ScriptableEntity EntityInfo; // { get; protected set; }
        
    // ------ GRID STATE INFO
    [HideInInspector] public Cell currentCell;
    [HideInInspector] public bool isSelected;
    
    // ------ STATS
    [SerializeField, Foldout("--- Stats ---")] private int movementRange;
    [HideInInspector] public int currentMovement;
    
    // ------- EVENTS
    public readonly UnityEvent EntitySelected = new();
    public static readonly UnityEvent OnEntityMove = new();
    
    // ------- MOVEMENT
    public abstract void MoveTowards(Cell cellToMove, bool blink = false);
    public void ResetMovement() => currentMovement = movementRange;
}

public interface IEntity
{
    void InitializeEntity(ScriptableEntity entityInfo);
}
