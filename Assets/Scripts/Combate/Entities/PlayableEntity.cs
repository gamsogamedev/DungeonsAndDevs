using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableEntity : BaseEntity, IEntity
{
    public ScriptableEntity_Playable PlayableInfo;

    public Skill skill1 { get; private set; }
    public Skill skill2 { get; private set; }
    public Skill skill3 { get; private set; }
    public Skill skill4 { get; private set; }

    public override void InitializeEntity(ScriptableEntity entityInfo)
    {
        base.InitializeEntity(entityInfo);
        PlayableInfo = entityInfo.ToPlayable();
        
        skill1 = new Skill(PlayableInfo.skill1, this);
        skill2 = new Skill(PlayableInfo.skill2, this);
        skill3 = new Skill(PlayableInfo.skill3, this);
        skill4 = new Skill(PlayableInfo.skill4, this);
        
        FixSort(Vector2Int.right);
    }
    
    internal override void Start()
    {
        base.Start();
        ResetMovement();
    }
}
