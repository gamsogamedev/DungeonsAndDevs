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

    public override void InitializeEntity(ScriptableEntity entity)
    {
        base.InitializeEntity(entity);
        HostileInfo = EntityInfo.ToHostile();
    }

    public override void StartTurn()
    {
        base.StartTurn();
        Invoke(nameof(TakeAction), 1f);
    }

    public void TakeAction()
    {
        var targetsInRange = GetTargetsInRange();
        BaseEntity target;
        if (targetsInRange.Any())
        {
            AttackTargetInRange();
        }
        else
        {
            target = CombatBehaviour.GetTarget(this, HostileInfo.Behaviour);
        
            OnEntityMoved.AddListener(AttackTargetInRange);
            ApproachTarget(target);   
        }
    }

    private void ApproachTarget(BaseEntity target)
    {
        var movementRadius = GridController.GetRadius(this.currentCell, currentMovement);
        Cell nearestToTarget = currentCell; 
        int nearestDistance = Int32.MaxValue;
        
        foreach (var cell in movementRadius)
        {
            if (cell._entityInCell is not null) continue;
            
            var cellDist = GridController.Distance(cell, target.currentCell);
            if (cellDist > .5f && cellDist < nearestDistance)
            {
                nearestToTarget = cell;
                nearestDistance = cellDist;
            }
        }
        
        MoveTowards(nearestToTarget);
    }

    private List<BaseEntity> GetTargetsInRange()
    {
        var attackRadius = HostileInfo.basicAttackRange.GetRange(currentCell);
        var targetsInRange = attackRadius
            .Select(c => c._entityInCell)
            .Where(c => c is not null)
            .Where(c => c.EntityInfo.entityType == EntityType.Playable)
            .ToList();

        return targetsInRange;
    }

    private void AttackTargetInRange()
    {
        var targetsInRange = GetTargetsInRange();
        if (!targetsInRange.Any())
        {
            Invoke(nameof(CallNextTurn), 1f);
            return;
        }
        
        var finalTarget = targetsInRange.OrderBy(c => Random.value).First();

        var targetDir = finalTarget.currentCell.cellCoord - currentCell.cellCoord;
        FixSort(targetDir);
        
        finalTarget.DoDamage(HostileInfo.basicAttackDamage);
        
        Invoke(nameof(CallNextTurn), 1f);
    }

    private void CallNextTurn()
    {
        OnEntityMoved.RemoveListener(AttackTargetInRange);
        CombatManager.Instance.NextTurn();
    }
}
