using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEntity))]
public class DraggableEntity : MonoBehaviour
{
    private BaseEntity _entity;
    
    private Vector3 positionBeforeDrag;
    private bool isDragging;

    private void Start()
    {
        _entity = GetComponent<BaseEntity>();
        
        isDragging = false;
        positionBeforeDrag = transform.position;
        
        CombatManager.OnStagePass.AddListener(DisableDrag);
    }

    private void DisableDrag(CombatState state)
    {
        CombatManager.OnStagePass.RemoveListener(DisableDrag);
        this.GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;
        
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

            if (_entity.currentCell is not null) 
                _entity.currentCell._entityInCell = null;
            _entity.currentCell = tileHit;
            
            tileHit._entityInCell = _entity;
            
            positionBeforeDrag = transform.position;
        }
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
        {
            transform.SetParent(null);
            this.GetComponent<Collider2D>().enabled = false;
            positionBeforeDrag = transform.position;
            isDragging = true;
        }

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 9);
    }
}
