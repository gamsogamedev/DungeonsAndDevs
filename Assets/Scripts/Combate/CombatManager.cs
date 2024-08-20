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

    public static Skill SelectedSkill;

    private void Awake() => Instance = this;
    
    private void Start()
    {
        OnEntitySelected.AddListener(SelectEntity);
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
    }

    private void SelectEntity(BaseEntity entity)
    {
        SelectedEntity = entity;

        PlayableEntity playable = (PlayableEntity) SelectedEntity;
         
        switch (currentStage)
        {
            case CombatState.PlayerWalk:
                GridManager.ShowRadiusAsWalkable(SelectedEntity.currentCell, SelectedEntity.currentMovement);
                break;
            case CombatState.PlayerAttack:
                Debug.Log($"Setting up {SelectedEntity.gameObject.name}'s skills");
                playable.skill1.OnSkillSelected.AddListener(() => SetupSkill(playable.skill1));
                playable.skill2.OnSkillSelected.AddListener(() => SetupSkill(playable.skill2));
                playable.skill3.OnSkillSelected.AddListener(() => SetupSkill(playable.skill3));
                playable.skill4.OnSkillSelected.AddListener(() => SetupSkill(playable.skill4));
                break;
        }
    }

    private void SetupSkill(Skill skill)
    {
        SelectedSkill = skill;
        
        SelectedSkill.OnSkillUse.AddListener((cell) => SelectedSkill.CastSkill(cell));
        SelectedSkill.OnSkillComplete.AddListener(delegate
        {
            GridManager.GridClear?.Invoke();
            
            SelectedSkill.OnSkillUse.RemoveAllListeners();
            SelectedSkill.OnSkillComplete.RemoveAllListeners();
            
            SelectedSkill = null;
        });
        
        GridManager.ShowRadiusAsRange(SelectedEntity.currentCell, skill.SkillRange);
    }
    
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (SelectedEntity is null) return;
        
        SelectedEntity.isSelected = false;
        SelectedEntity = null;
    }
}
