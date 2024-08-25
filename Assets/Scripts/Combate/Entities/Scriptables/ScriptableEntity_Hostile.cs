using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/New Hostile", fileName = "Hostile")]
public class ScriptableEntity_Hostile : ScriptableEntity
{
    public override ScriptableEntity_Playable ToPlayable() => null; 
    public override ScriptableEntity_Hostile ToHostile() => this; 
}
