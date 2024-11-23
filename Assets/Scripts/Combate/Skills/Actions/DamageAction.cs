using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DamageAction : ICombatAction
{
    public Range areaOfEffect;
    
    public int damage;

    public bool useDirection;
    public bool collides;
    
    [Space(10)] public SkillFX skillVisuals;
    
    public Cell ExecuteAction(BaseEntity caster, Cell target)
    {
        var aoe = areaOfEffect.GetRange(target);
        BaseEntity entity;

        //caster.currentCell.ActivateVisual(skillVisuals.entityCasterFX, skillVisuals.hasCasterFX);
        
        var finalTarget = target;
        if (useDirection)
        {
            var path = GridController.GetPath(caster.currentCell, target, stateFilter: CellState.Idle | CellState.Range);
            
            var direction = caster.currentCell.cellCoord - path[0].cellCoord;
            var rotation = 0;
            if (direction.x != 0)
            {
                rotation = (direction.x < 0) ? 0 : 180;
            }
            else
            {
                rotation = (direction.y < 0) ? 75 : -105;
            }
             
            path.First().ActivateVisual(skillVisuals.manyCellFX_nearCaster, skillVisuals.hasManyCellFX, rotation);
            path.Remove(path.First());
            
            foreach (var pathCell in path)
            {
                if (collides && pathCell._entityInCell is not null || pathCell == path.Last())
                {
                    finalTarget = pathCell;
                    break;
                }
                pathCell.ActivateVisual(skillVisuals.manyCellFX_midway, skillVisuals.hasManyCellFX, rotation);
            }
            
            path.Last().ActivateVisual(skillVisuals.manyCellFX_nearTarget, skillVisuals.hasManyCellFX, rotation);
        }
        
        //finalTarget.ActivateVisual(skillVisuals.singleCellFX, skillVisuals.hasSingleCellFX);
        foreach (var cell in aoe)
        {
            // Debug.Log($"Aplicando {damage} de dano em {cell.name}");
            if ((entity = cell._entityInCell) is not null)
            {
                entity.currentCell.ActivateVisual(skillVisuals.entityTargetFX, skillVisuals.hasTargetFX);
                entity.DoDamage(damage);
            }
        }

        return finalTarget;
    }

    public List<Cell> PreviewRange(Cell center)
    {
        return areaOfEffect.GetRange(center);
    }
}
