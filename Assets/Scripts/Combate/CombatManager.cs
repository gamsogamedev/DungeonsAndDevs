using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum CombatState {Setup, Neutral, Movement, Attack}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public static BaseEntity SelectedEntity;
    public static readonly UnityEvent<BaseEntity> OnEntityTurn = new();


    public static readonly UnityEvent<BaseEntity> EnableMovement = new();
    public static readonly UnityEvent CastComplete = new();
    
    public static Skill SelectedSkill;
    
    private void Awake()
    { 
        Instance = this;
        
        SelectedEntity = null;
        SelectedSkill = null;
        currentStage = CombatState.Setup;
    }
    
    private void SetCurrentEntity(BaseEntity entity)
    {
        GridManager.GridClear?.Invoke();
        SetCombatStage(CombatState.Neutral);
        
        SelectedEntity = entity;
        SelectedEntity.ResetMovement();

        SelectedSkill = null;
        PlayableEntity playable = (PlayableEntity) SelectedEntity;
        playable.skill1.OnSkillSelected.AddListener(() => SetAttackStage(playable.skill1));
        playable.skill2.OnSkillSelected.AddListener(() => SetAttackStage(playable.skill2));
        playable.skill3.OnSkillSelected.AddListener(() => SetAttackStage(playable.skill3));
        playable.skill4.OnSkillSelected.AddListener(() => SetAttackStage(playable.skill4));

        OnEntityTurn?.Invoke(SelectedEntity);
    }
    
    private void SetMovementStage(BaseEntity entity)
    {
        if (entity != SelectedEntity) return;
        
        SetCombatStage(CombatState.Movement);
        
        GridManager.GridClear?.Invoke();
        GridManager.ShowRadiusAsWalkable(SelectedEntity.currentCell, SelectedEntity.currentMovement);
    }

    private void SetAttackStage(Skill skill)
    {
        SetCombatStage(CombatState.Attack);
        
        SelectedSkill = skill;
        
        SelectedSkill.OnSkillUse.AddListener((cell) => SelectedSkill.CastSkill(cell));
        SelectedSkill.OnSkillComplete.AddListener(delegate
        {
            GridManager.GridClear?.Invoke();
            
            SelectedSkill.OnSkillUse?.RemoveAllListeners();
            SelectedSkill.OnSkillComplete?.RemoveAllListeners();
            SelectedSkill = null;
            
            CastComplete?.Invoke();
        });
        
        GridManager.GridClear?.Invoke();
        GridManager.ShowRadiusAsRange(SelectedEntity.currentCell, skill.SkillRange);
    }
    
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (SelectedEntity is null) return;
        if (SelectedSkill is not null) SelectedSkill = null;
            
        SetCombatStage(CombatState.Neutral);
        GridManager.GridClear?.Invoke();
    }

    #region Turn Order

    private List<BaseEntity> _turnOrder;
    private int orderCount;
    private List<BaseEntity> SetTurnOrder(List<BaseEntity> entitiesInvolved)
    {
        var orderList = new BaseEntity[entitiesInvolved.Count];
     
        foreach (var ent in entitiesInvolved)
        {
            var valid = false;
            while (!valid)
            {
                var ord = Random.Range(0, entitiesInvolved.Count);
                if (orderList[ord] is not null) continue;
                
                valid = true;
                orderList[ord] = ent;
            }
        }

        return orderList.ToList();
    }

    public void NextTurn(bool startCombat = false)
    {
        if (startCombat)
        {
            SetCurrentEntity(_turnOrder[0]);
            return;
        }
        
        orderCount = (orderCount + 1 >= _turnOrder.Count) ? 0 : orderCount + 1;
        SetCurrentEntity(_turnOrder[orderCount]);
    }

    #endregion
    
    #region Stage Management

    public CombatState currentStage;
    public static readonly UnityEvent<CombatState> OnStagePass = new();
    public void SetCombatStage(CombatState nxtStage)
    {
        currentStage = nxtStage;
        OnStagePass?.Invoke(currentStage);
    }

    public void FinishPositionSetup()
    {
        EnableMovement.AddListener(SetMovementStage);
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
        
        var all = GridController.GetEntitiesOnGrid();
        _turnOrder = SetTurnOrder(all);
        NextTurn(true);
    }

    #endregion
}
