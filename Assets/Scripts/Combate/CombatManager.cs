using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    private BaseEntity _selectedEntity;
    
    public static readonly UnityEvent<BaseEntity> OnEntitySelected = new();

    private void Start()
    {
        OnEntitySelected.AddListener(SelectEntity);
    }

    private void SelectEntity(BaseEntity entity)
    {
        _selectedEntity = entity;
        GridManager.Instance.ShowRadius(entity.currentCell, entity.movementRange);
    }
}
