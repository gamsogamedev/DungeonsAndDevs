using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class MoveAction : ICombatAction
{
    public Range directionMoved;
    
    public Cell ExecuteAction(BaseEntity caster, Cell target)
    {
        var pathPreview = directionMoved.GetRange(caster.currentCell);
        Cell cellToMove = caster.currentCell;
     
        foreach (var cell in pathPreview)
        {
            if (cell._entityInCell is not null) break;
            cell.SetCellAsWalkable();
            cellToMove = cell;
        }
        
        if (cellToMove != caster.currentCell)
            caster.MoveTowards(cellToMove, blink: true);   

        return cellToMove;
    }

    public List<Cell> PreviewRange(Cell center)
    {
        return new List<Cell>() {center};
    }
}
