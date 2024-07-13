using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CombatState {Neutral, PlayerWalk, PlayerAttack, EnemyWalk, EnemyAttack}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public CombatState currentStage;
    
    public static BaseEntity SelectedEntity;
    public static readonly UnityEvent<BaseEntity> OnEntitySelected = new();

    private void Awake() => Instance = this;
    
    private Image cooldownOverlay;

    private void Start()
    {
        OnEntitySelected.AddListener(SelectEntity);
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
    }

    private void SelectEntity(BaseEntity entity)
    {
        SelectedEntity = entity;

        if (currentStage == CombatState.PlayerWalk) // TEMPORARY
        {
            GridManager.Instance.ShowRadius(entity.currentCell, entity.currentMovement);
        }
    }
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (SelectedEntity is null) return;
        
        SelectedEntity.isSelected = false;
        SelectedEntity = null;
    }
}
