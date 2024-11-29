using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private SpriteRenderer visualRef;
    private bool facingRight;
    private int baseSortOrder;
    
    internal virtual void Start()
    {
        currentHealth = Health;
    }

    public virtual void InitializeEntity(ScriptableEntity entity)
    {
        EntityInfo = entity;
        
        visualRef = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        visualRef.sprite = EntityInfo.entityVisuals;
        baseSortOrder = visualRef.sortingOrder;
        
        GetComponentInChildren<HealthHUD>().Init(this);
    }

    // ------ STATS
    private int Health => EntityInfo.GetMaxHealth();
    public int currentHealth { get; private set; }
    public float healthPercentage => (float)currentHealth / Health;

    public readonly UnityEvent<float> HealthUpdated = new();
    
    public void DoDamage(int amount)
    {
        currentHealth -= amount;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HealthUpdated?.Invoke(0f);
            CombatManager.OnEntityDeath?.Invoke(this);

            Invoke(nameof(ProccessDeath), 1f);
        }

        HealthUpdated?.Invoke(healthPercentage);
    }

    private void ProccessDeath()
    {
        var visuals = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        var hudVisuals = transform.Find("HUD").GetComponent<CanvasGroup>();

        var deleteSequence = DOTween.Sequence();
        deleteSequence.Append(visuals.DOFade(0f, .2f));
        deleteSequence.Join(hudVisuals.DOFade(0f, .2f));
        deleteSequence.AppendCallback(() => visuals.enabled = false);

        currentCell._entityInCell = null;
        Destroy(this.gameObject, 1.5f);
    }
    
    private int MovementRange => EntityInfo.GetSpeed();
    [HideInInspector] public int currentMovement;
    
    // ------- EVENTS
    public readonly UnityEvent EntitySelected = new();
    public readonly UnityEvent OnEntityMoved = new();

    public virtual void StartTurn()
    {
        Debug.Log($"{gameObject.name}'s Turn");
        GetComponentInChildren<TurnHighlight>().HighlightEntity();
    }
    
    // ------- POSITION
    public void SetPosition(Cell c)
    {
        transform.SetParent(c.transform);
        transform.localPosition = Vector3.zero;

        this.currentCell = c;
        c._entityInCell = this;
        
        FixSort(EntityInfo.entityType == EntityType.Playable ? Vector2Int.right : Vector2Int.left);
    }

    public void FixSort(Vector2Int lastDirection)
    {
        facingRight = lastDirection.x > 0;
        visualRef.flipX = facingRight;

        if (currentCell is null) return;
        visualRef.sortingOrder = baseSortOrder - currentCell.cellCoord.y;
    }
    
    // ------- MOVEMENT
    public abstract void MoveTowards(Cell cellToMove, bool blink = false);
    public void ResetMovement() => currentMovement = MovementRange;
}

public interface IEntity
{
    void InitializeEntity(ScriptableEntity entityInfo);
}
