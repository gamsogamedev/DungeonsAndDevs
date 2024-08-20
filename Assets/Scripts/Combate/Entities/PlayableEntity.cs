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

    public void InitializeEntity(ScriptableEntity entityInfo)
    {
        EntityInfo = entityInfo;
        PlayableInfo = entityInfo.ToPlayable();
        
        skill1 = new Skill(PlayableInfo.skill1, this);
        skill2 = new Skill(PlayableInfo.skill2, this);
        skill3 = new Skill(PlayableInfo.skill3, this);
        skill4 = new Skill(PlayableInfo.skill4, this);
    }

    private void OnEnable()
    {
        InitializeEntity(PlayableInfo);
    }

    // TODO -- stat de destreza influencia o range de movimento
    private Vector3 positionBeforeDrag;
    private bool isDragging, dragEnabled;
    
    private void Start()
    {
        isDragging = false;
        dragEnabled = true;
        positionBeforeDrag = transform.position;
        
        ResetMovement();
        EntitySelected.AddListener(SelectEntity);
        OnEntityMove.AddListener(() => isSelected = false); // Refactor this later (won't work for multiple entities)
    }

    #region Temporary
    private void OnMouseUp()
    {
        if (!isDragging) return;
        
        var hit = Physics2D.OverlapBox(transform.position, Vector3.one / 20f, 0);
        this.GetComponent<Collider2D>().enabled = true;
        
        if (hit is null || !hit.CompareTag("Cell"))
        {
            transform.position = positionBeforeDrag;
            return;
        }

        var tileHit = hit.GetComponent<Cell>();
        if (tileHit._entityInCell)
        {
            transform.position = positionBeforeDrag;
            return;
        }
        else
        {
            transform.SetParent(tileHit.transform);
            transform.localPosition = Vector3.zero;
            currentCell = tileHit;
            tileHit._entityInCell = this;

            positionBeforeDrag = transform.position;
            this.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnMouseDrag()
    {
        if (!dragEnabled) return;
        
        if (!isDragging)
        {
            transform.SetParent(null);
            this.GetComponent<Collider2D>().enabled = false;
            positionBeforeDrag = transform.position;
            isDragging = true;
        }

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 9);
    }
    
    #endregion
    
    private void SelectEntity()
    {
        if (!isSelected) CombatManager.OnEntitySelected?.Invoke(this);
        isSelected = !isSelected;
    }

    public override void MoveTowards(Cell cellToMove) => StartCoroutine(Move(cellToMove));
    
    private IEnumerator Move(Cell cellToMove)
    {
        Debug.Log(cellToMove);
        var path = GridController.GetPath(currentCell, cellToMove);
        
        foreach (var cell in path)
        {
            var moveSequence = DOTween.Sequence();
            moveSequence.AppendCallback(() => transform.SetParent(cell.transform));
            moveSequence.Append(transform.DOLocalMove(Vector3.zero, .2f));
            moveSequence.AppendCallback(delegate
            {
                currentCell = cell;
                cell._entityInCell = this;
                currentMovement--;
            });
            yield return new WaitForSeconds(0.25f);
        }
        
        OnEntityMove?.Invoke();
    }
}
