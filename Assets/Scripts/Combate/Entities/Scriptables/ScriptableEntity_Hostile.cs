using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/New Hostile", fileName = "Hostile")]
public class ScriptableEntity_Hostile : ScriptableEntity
{
    public override ScriptableEntity_Playable ToPlayable() => null; 
    public override ScriptableEntity_Hostile ToHostile() => this;

    public CombatBehaviour.TargetBehavior Behaviour;
    
    public Range basicAttackRange;
    public int basicAttackDamage;

    public override BaseEntity GenerateEntity()
    {
        var hostile = Instantiate(entityPrefab) as HostileEntity;
        hostile?.InitializeEntity(this);
        return hostile;
    }
}
