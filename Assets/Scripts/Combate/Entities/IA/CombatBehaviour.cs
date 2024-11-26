using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CombatBehaviour
{
    public enum TargetBehavior
    {
        Random,
        Closest,
        Farthest,
        Isolated,
        Grouped,
        MostHP,
        LeastHP
    }

    // TODO: Implement other strategies later
    public static BaseEntity GetTarget(BaseEntity ent, TargetBehavior bhv)
    {
        var allTargets = GridController.GetEntitiesOnGrid(EntityType.Playable);
        BaseEntity finalTarget;
        
        switch (bhv)
        {
            case TargetBehavior.Closest:
                finalTarget = allTargets.OrderBy(t => GridController.Distance(ent.currentCell, t.currentCell)).First();
                break;
            case TargetBehavior.Farthest:
                finalTarget = allTargets.OrderBy(t => GridController.Distance(ent.currentCell, t.currentCell)).Last();
                break;
            default:
                finalTarget = allTargets.OrderBy(t => Random.value).First();
                break;
        }
        
        return finalTarget;
    }
}
