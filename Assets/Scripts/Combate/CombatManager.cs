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

    public static ScriptableSkill SelectedSkill;

    private void Awake() => Instance = this;
    
    // private Image cooldownOverlay;

    private void Start()
    {
        OnEntitySelected.AddListener(SelectEntity);
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
    }

    private void SelectEntity(BaseEntity entity)
    {
        SelectedEntity = entity;
        
        var playableEntity = SelectedEntity.EntityInfo.ToPlayable();
        //var hostileEntity = SelectedEntity.EntityInfo.To
        
        switch (currentStage)
        {
            case CombatState.PlayerWalk when playableEntity is not null:
                GridManager.ShowRadiusAsWalkable(SelectedEntity.currentCell, SelectedEntity.currentMovement);
                break;
            case CombatState.PlayerAttack when playableEntity is not null:
                Debug.Log($"Setting up {SelectedEntity.gameObject.name}'s skills");
                playableEntity.skill1.OnSkillSelected.AddListener(() => SetupSkill(playableEntity.skill1));
                playableEntity.skill2.OnSkillSelected.AddListener(() => SetupSkill(playableEntity.skill2));
                playableEntity.skill3.OnSkillSelected.AddListener(() => SetupSkill(playableEntity.skill3));
                playableEntity.skill4.OnSkillSelected.AddListener(() => SetupSkill(playableEntity.skill4));
                break;
        }
    }

    private void SetupSkill(ScriptableSkill skill)
    {
        SelectedSkill = skill;
        
        SelectedSkill.OnSkillUse.AddListener((cell) => SelectedSkill.CastSkill(new CellTarget(cell)));
        SelectedSkill.OnSkillComplete.AddListener(delegate
        {
            GridManager.GridClear?.Invoke();
            
            SelectedSkill.OnSkillUse.RemoveAllListeners();
            SelectedSkill.OnSkillComplete.RemoveAllListeners();
            
            SelectedSkill = null;
        });
        
        GridManager.ShowRadiusAsRange(SelectedEntity.currentCell, skill.skillRange);
    }
    
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (SelectedEntity is null) return;
        
        SelectedEntity.isSelected = false;
        SelectedEntity = null;
    }
}
