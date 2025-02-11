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

        var finalTarget = target;
        if (useDirection)
        {
            // Get the path for the caster (assuming single-tile entity for simplicity)
            var path = Pathfinder.GetPath(
                caster.currentCell,
                target,
                stateFilter: CellState.Idle | CellState.Range,
                avoidEntities: false,
                entityWidth: 1, // Assuming caster is single-tile
                entityHeight: 1 // Assuming caster is single-tile
            );

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

            foreach (var pathCell in path)
            {
                if (collides && pathCell._entityInCell is not null || pathCell == path.Last())
                {
                    finalTarget = pathCell;
                    break;
                }

                pathCell.ActivateVisual(
                    pathCell == path[0] ? skillVisuals.manyCellFX_nearCaster : skillVisuals.manyCellFX_midway,
                    skillVisuals.hasManyCellFX, rotation);
            }

            finalTarget.ActivateVisual(skillVisuals.manyCellFX_nearTarget, skillVisuals.hasManyCellFX, rotation);
        }

        // Track entities that have already been damaged to avoid duplicate damage
        HashSet<BaseEntity> damagedEntities = new HashSet<BaseEntity>();

        foreach (var cell in aoe)
        {
            if ((entity = cell._entityInCell) is not null && !damagedEntities.Contains(entity))
            {
                // Apply visual effects to the target cell (skill visuals, not entity visuals)
                cell.ActivateVisual(skillVisuals.entityTargetFX, skillVisuals.hasTargetFX);

                // Apply damage to the entity
                entity.DoDamage(damage);
                damagedEntities.Add(entity); // Mark the entity as damaged
            }
        }

        return finalTarget;
    }

    public List<Cell> PreviewRange(Cell center)
    {
        return areaOfEffect.GetRange(center);
    }
}
