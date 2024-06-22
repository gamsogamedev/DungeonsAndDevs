using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableEntity : BaseEntity
{
    // TODO -- stat de destreza influencia o range de movimento
    private Vector3 positionBeforeDrag;
    private bool isDragging;

    private void Start()
    {
        isDragging = false;
        positionBeforeDrag = transform.position;
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
    
    #endregion

    private void OnMouseDown()
    {
        CombatManager.OnEntitySelected?.Invoke(this);
    }
}
