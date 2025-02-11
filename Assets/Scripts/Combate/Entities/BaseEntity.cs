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

    public Vector2Int entitySize = Vector2Int.one; // Default to 1x1
    private List<Cell> occupiedCells = new List<Cell>();

    [SerializeField] private SpriteRenderer visualRef;
    private bool facingRight;
    private int baseSortOrder;

    internal virtual void Start()
    {
        currentHealth = Health;
    }

    public virtual void InitializeEntity(ScriptableEntity entity)
    {
        EntityInfo = entity;

        currentCell = null;

        visualRef.sprite = EntityInfo.entityVisuals;
        baseSortOrder = visualRef.sortingOrder;

        currentHealth = Health;
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
        //var visuals = transform.Find("Visuals").GetComponent<SpriteRenderer>();
        var hudVisuals = transform.Find("HUD").GetComponent<CanvasGroup>();

        var deleteSequence = DOTween.Sequence();
        deleteSequence.Append(visualRef.DOFade(0f, .2f));
        deleteSequence.Join(hudVisuals.DOFade(0f, .2f));
        deleteSequence.AppendCallback(() => visualRef.enabled = false);

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
        GetComponentInChildren<TurnHighlight>().HighlightEntity();
    }

    // ------- POSITION
    public void SetPosition(Cell primaryCell)
    {
        // Clear previous cells
        foreach (var cell in occupiedCells)
        {
            if (cell != null) cell._entityInCell = null;
        }
        occupiedCells.Clear();

        // Calculate occupied cells based on size
        for (int x = 0; x < EntityInfo.width; x++)
        {
            for (int y = 0; y < EntityInfo.height; y++)
            {
                var cellCoord = primaryCell.cellCoord + new Vector2Int(x, y);
                var cell = GridController.GetCellAt(cellCoord);
                if (cell != null && cell._entityInCell == null)
                {
                    cell._entityInCell = this;
                    occupiedCells.Add(cell);
                }
                else
                {
                    // Handle invalid placement (e.g., overlapping or out-of-bounds)
                    throw new InvalidOperationException("Invalid placement for multi-tile entity.");
                }
            }
        }

        // Set transform position to the primary cell
        transform.SetParent(primaryCell.transform);
        transform.localPosition = Vector3.zero;
        currentCell = primaryCell;
    }

    public void FixSort(Vector2Int lastDirection)
    {
        facingRight = lastDirection.x >= 0;

        visualRef.flipX = facingRight;

        if (currentCell is null) return;
        visualRef.sortingOrder = baseSortOrder - currentCell.cellCoord.y;
    }

    public void MoveTowards(Cell targetCell, bool blink = false)
    {
        var path = Pathfinder.GetPath(currentCell, targetCell, CellState.Walkable, true, EntityInfo.width, EntityInfo.height);

        // Check if the target position is valid for the entity's size
        if (!IsPositionValid(targetCell))
        {
            Debug.LogWarning("Alguma cell da entidade estava colidindo com outra entidade");
            return;
        }

        if (blink)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => SetPosition(targetCell)); // Pass the target cell, not the path
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .5f));
            moveSequence.AppendCallback(() => currentMovement--);
            return;
        }

        StartCoroutine(Move(path));
    }

    private bool IsPositionValid(Cell primaryCell)
    {
        for (int x = 0; x < EntityInfo.width; x++)
        {
            for (int y = 0; y < EntityInfo.height; y++)
            {
                var cellCoord = primaryCell.cellCoord + new Vector2Int(x, y);
                var cell = GridController.GetCellAt(cellCoord);
                if (cell == null || (cell._entityInCell != null && cell._entityInCell != this))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private IEnumerator Move(List<Cell> path)
    {
        if (path == null || path.Count == 0) yield break;

        // Get the target cell (last cell in the path)
        var targetCell = path[path.Count - 1];

        // Move the entity to the target cell
        var moveSequence = DOTween.Sequence();
        moveSequence.AppendCallback(() => SetPosition(targetCell)); // Update all occupied cells
        moveSequence.Append(transform.DOLocalMove(Vector3.zero, 0.2f * path.Count)); // Adjust duration based on path length
        moveSequence.AppendCallback(() =>
        {
            currentMovement--;
            OnEntityMoved?.Invoke();
        });

        yield return moveSequence.WaitForCompletion();
    }

    public void ResetMovement() => currentMovement = MovementRange;
}

public interface IEntity
{
    void InitializeEntity(ScriptableEntity entityInfo);
}
