using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum PlayableClass
{
    None,
    Programador,
    Artista,
    GameDesigner,
    Musico
}

[CreateAssetMenu(menuName = "Entity/New Playable", fileName = "Playable")]
public class ScriptableEntity_Playable : ScriptableEntity
{
    public override ScriptableEntity_Playable ToPlayable() => this; 
    public override ScriptableEntity_Hostile ToHostile() => null;
    public override BaseEntity GenerateEntity()
    {
        var playable = Instantiate(entityPrefab) as PlayableEntity;
        playable?.InitializeEntity(this);

        return playable;
    }

    [Header("Classe")] 
    public PlayableClass classe;

    [Header("Skills")]
    [Expandable] public ScriptableSkill skill1;
    [Expandable] public ScriptableSkill skill2;
    [Expandable] public ScriptableSkill skill3;
    [Expandable] public ScriptableSkill skill4;
}
