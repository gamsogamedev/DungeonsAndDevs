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
    }

    // Remove later
    private void OnEnable()
    {
        InitializeEntity(PlayableInfo);
    }

    internal override void Start()
    {
        base.Start();
        ResetMovement();
    }

    public override void MoveTowards(Cell cellToMove, bool blink = false) 
    {
        if (blink)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => transform.SetParent(cellToMove.transform));
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .5f));
            moveSequence.AppendCallback(delegate
            {
                currentCell = cellToMove;
                cellToMove._entityInCell = this;
                currentMovement--;
            });
            return;
        }
        StartCoroutine(Move(cellToMove));
    }
    
    private IEnumerator Move(Cell cellToMove)
    {
        var path = GridController.GetPath(currentCell, cellToMove);
        foreach (var cell in path)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => transform.SetParent(cell.transform));
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .2f));
            moveSequence.AppendCallback(delegate
            {
                currentCell._entityInCell = null;
                currentCell = cell;
                cell._entityInCell = this;
                currentMovement--;
            });
            yield return new WaitForSeconds(0.25f);
        }
        
        OnEntityMoved?.Invoke();
    }
}
