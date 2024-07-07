using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CombatState {Neutral, PlayerWalk, PlayerAttack, EnemyWalk, EnemyAttack}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public static CombatState currentStage;
    
    public static BaseEntity SelectedEntity;
    public static readonly UnityEvent<BaseEntity> OnEntitySelected = new();

    private void Awake() => Instance = this;

    private void Start()
    {
        currentStage = CombatState.Neutral;
        OnEntitySelected.AddListener(SelectEntity);
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
    }

    private void SelectEntity(BaseEntity entity)
    {
        SelectedEntity = entity;
        GridManager.Instance.ShowRadius(entity.currentCell, entity.currentMovement);
    }
    
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (SelectedEntity is null) return;
        
        SelectedEntity.isSelected = false;
        SelectedEntity = null;
    }
}
