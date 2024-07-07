using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableEntity : BaseEntity
{
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
    
    public override void MoveTowards(Cell cellToMove)
    {
        transform.SetParent(cellToMove.transform);
        transform.localPosition = Vector3.zero;
        currentCell = cellToMove;
        cellToMove._entityInCell = this;

        currentMovement--;
        OnEntityMove?.Invoke();
    }
}
