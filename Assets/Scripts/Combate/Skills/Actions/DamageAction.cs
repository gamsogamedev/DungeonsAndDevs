using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DamageAction : ICombatAction
{
    public Range areaOfEffect;
    
    public float damage;
    
    public Cell ExecuteAction(BaseEntity caster, Cell target)
    {
        var aoe = areaOfEffect.GetRange(target);
        BaseEntity entity;
        
        foreach (var cell in aoe)
        {
            Debug.Log($"Aplicando {damage} de dano em {cell.name}");
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
