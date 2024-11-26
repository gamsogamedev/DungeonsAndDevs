using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HostileEntity : BaseEntity
{
    public ScriptableEntity_Hostile HostileInfo;

    private void OnEnable()
    {
        InitializeEntity(HostileInfo);
    }

    public override void InitializeEntity(ScriptableEntity entity)
    {
        base.InitializeEntity(entity);
        HostileInfo = EntityInfo.ToHostile();
    }

    public override void StartTurn()
    {
        base.StartTurn();
        var target = CombatBehaviour.GetTarget(this, HostileInfo.Behaviour);
        
        OnEntityMoved.AddListener(AttackTargetInRange);
        ApproachTarget(target);

        Invoke(nameof(CallNextTurn), 1f);
    }

    private void ApproachTarget(BaseEntity target)
    {
        var movementRadius = GridController.GetRadius(this.currentCell, currentMovement);
        Cell nearestToTarget = currentCell; 
        int nearestDistance = Int32.MaxValue;
        
        foreach (var cell in movementRadius)
        {
            var cellDist = GridController.Distance(cell, target.currentCell);
            if (cellDist > .5f && cellDist < nearestDistance)
            {
                nearestToTarget = cell;
                nearestDistance = cellDist;
            }
        }
        
        MoveTowards(nearestToTarget);
    }

    private void AttackTargetInRange()
    {
        var attackRadius = HostileInfo.basicAttackRange.GetRange(currentCell);
        var targetsInRange = attackRadius
            .Where(c => c._entityInCell is not null)
            .Where(c => c._entityInCell.EntityInfo.entityType == EntityType.Playable)
            .ToList();

        if (!targetsInRange.Any()) return;
        
        var finalTarget = targetsInRange.OrderBy(c => Random.value).First();
        finalTarget._entityInCell.DoDamage(HostileInfo.basicAttackDamage);
    }

    private void CallNextTurn()
    {
        OnEntityMoved.RemoveListener(AttackTargetInRange);
        CombatManager.Instance.NextTurn();
    }
    
    // ----- MOVEMENT -------------------------------------
    public override void MoveTowards(Cell cellToMove, bool blink = false) 
    {
        if (blink)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => transform.SetParent(cellToMove.transform));
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .5f));
            moveSequence.AppendCallback(delegate
            {
                currentCell = cellToMove;
                cellToMove._entityInCell = this;
                currentMovement--;
            });
            return;
        }
        StartCoroutine(Move(cellToMove));
    }
    
    private IEnumerator Move(Cell cellToMove)
    {
        var path = GridController.GetPath(currentCell, cellToMove, CellState.Idle);
        foreach (var cell in path)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => transform.SetParent(cell.transform));
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .2f));
            moveSequence.AppendCallback(delegate
            {
                currentCell._entityInCell = null;
                currentCell = cell;
                cell._entityInCell = this;
                currentMovement--;
            });
            yield return new WaitForSeconds(0.25f);
        }
        
        OnEntityMoved?.Invoke();
    }
}
