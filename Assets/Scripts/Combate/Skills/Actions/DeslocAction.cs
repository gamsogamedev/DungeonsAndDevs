using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DeslocAction : ICombatAction
{
    public Range directionMoved;

    public Cell ExecuteAction(BaseEntity caster, Cell target)
    {
        var targetEntity = target._entityInCell;
        if (targetEntity is null) return target;
        
        var pathPreview = directionMoved.GetRange(targetEntity.currentCell);
        Cell cellToMove = targetEntity.currentCell;
        foreach (var cell in pathPreview)
        {
            if (cell._entityInCell is not null) break;
            cellToMove = cell;
        }

        if (cellToMove != targetEntity.currentCell)
            targetEntity.MoveTowards(cellToMove);
        
        return cellToMove;
    }

    public List<Cell> PreviewRange(Cell center)
    {
        return new List<Cell>();
    }
}
