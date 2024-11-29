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

    public bool isOnGrid { get; private set; }

    private void Awake()
    {
        GridManager.GridGenerated.AddListener(SetupDrag);
        CombatManager.OnStagePass.AddListener(DisableDrag);
    }

    public void SetupDrag()
    {
        _entity = GetComponent<BaseEntity>();
        
        positionBeforeDrag = new Vector3(-100, -100, -100);
        isOnGrid = false;
    }

    private void DisableDrag(CombatState state)
    {
        CombatManager.OnStagePass.RemoveListener(DisableDrag);
        this.GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }

    public void OnMouseUp()
    {
        if (!isDragging)
        {
            transform.position = positionBeforeDrag;
            return;
        }
        isDragging = false;
        
        var hit = Physics2D.OverlapBox(transform.position, Vector3.one / 20f, 0);
        this.GetComponent<Collider2D>().enabled = true;
        
        if (hit is null || !hit.CompareTag("Cell"))
        {
            if (!isOnGrid){
                transform.position = positionBeforeDrag;
                return;
            }
            transform.position = positionBeforeDrag;
            return;
        }

        var tileHit = hit.GetComponent<Cell>();
        if (tileHit._entityInCell)
        {
            if (!isOnGrid){
                transform.position = positionBeforeDrag;
                return;
            }
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
            
            isOnGrid = true;
            positionBeforeDrag = transform.position;
            _entity.FixSort(Vector2Int.right);
        }
    }
    
    public void OnMouseDrag()
    {
        if (!isDragging)
        {
            transform.SetParent(null);
            this.GetComponent<Collider2D>().enabled = false;
            isDragging = true;
        }

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3.forward * 9);
    }
}
