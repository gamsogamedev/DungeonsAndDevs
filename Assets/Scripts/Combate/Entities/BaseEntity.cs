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
    public ScriptableEntity EntityInfo; // { get; set; }
    public void InitializeEntity(ScriptableEntity entityInfo) => EntityInfo = entityInfo;
        
    // ------ GRID STATE INFO
    [HideInInspector] public Cell currentCell;
    [HideInInspector] public bool isSelected;
    
    // ------ STATS
    [SerializeField, Foldout("--- Stats ---")] private int movementRange;
    [HideInInspector] public int currentMovement;
    
    // ------- EVENTS
    public readonly UnityEvent EntitySelected = new();
    public static readonly UnityEvent OnEntityMove = new();

    public abstract void MoveTowards(Cell cellToMove);

    public void ResetMovement() => currentMovement = movementRange;
}
