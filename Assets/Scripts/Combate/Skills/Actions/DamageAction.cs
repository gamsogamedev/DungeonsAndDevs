using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DamageAction : ICombatAction
{
    [HideInInspector] public ITarget target;

    public Range areaOfEffect;
    
    public float damage;
    
    public override void ExecuteAction()
    {
        var aoe = areaOfEffect.GetRange(target.GetCell());
        BaseEntity entity;
        
        foreach (var cell in aoe)
        {
            if ((entity = cell._entityInCell) is not null)
            {
                // Deal Damage Here
            }
        }
        
        base.ExecuteAction();
    }
}
