using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/New Playable", fileName = "Playable")]
public class ScriptableEntity_Playable : ScriptableEntity
{
    public override ScriptableEntity_Playable ToPlayable() => this; 
    //public override ScriptableEntity_Playable ToHostile() => null; 
    
    [Expandable] public ScriptableSkill skill1;
    [Expandable] public ScriptableSkill skill2;
    [Expandable] public ScriptableSkill skill3;
    [Expandable] public ScriptableSkill skill4;
}
