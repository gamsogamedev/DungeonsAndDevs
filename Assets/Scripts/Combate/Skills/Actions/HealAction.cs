using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HealAction : ICombatAction
{
    public Range areaOfEffect;
    
    public float heal;
    
    public Cell ExecuteAction(BaseEntity caster, Cell target)
    {
        var aoe = areaOfEffect.GetRange(target);
        BaseEntity entity;
        
        foreach (var cell in aoe)
        {
            Debug.Log($"Aplicando {heal} de cura em {cell.name}");
            if ((entity = cell._entityInCell) is not null)
            {
                // Deal Damage Here
            }
        }

        return target;
    }

    public List<Cell> PreviewRange(Cell center)
    {
        return areaOfEffect.GetRange(center);
    }
}
