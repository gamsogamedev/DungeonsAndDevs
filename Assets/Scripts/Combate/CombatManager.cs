using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum CombatState {Setup, Playable, Enemy }
public enum TurnState {Neutral, Movement, Attack}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    
    public static readonly UnityEvent<BaseEntity> OnEntityTurn = new();

    [SerializeField] private CombatSettingsScriptable cSettings;
    private void LoadCombatSettings(CombatSettingsScriptable sett) => cSettings = sett;
    
    public class Turn
    {
        public BaseEntity TurnEntity;
        public Skill TurnUsedSkill;
        public bool UsedSkill;
        public TurnState CurrentState;

        public Turn(BaseEntity turnEntity)
        {
            this.TurnEntity = turnEntity;
            
            this.TurnUsedSkill = null;
            this.UsedSkill = false;
            
            this.TurnEntity.ResetMovement();
            
            CurrentState = TurnState.Neutral;
        }

        public void SetTurnState(TurnState state) => CurrentState = state;

        public void AssignSkill(Skill skill)
        {
            TurnUsedSkill = skill;
            TurnUsedSkill?.SetupSkill();
        }
    }
    public static Turn CurrentTurn { get; private set; }
    public static BaseEntity TurnEntity => CurrentTurn.TurnEntity;
    public static Skill TurnSkill => CurrentTurn.TurnUsedSkill;
    
    public static readonly UnityEvent ActionTaken = new();
    public static readonly UnityEvent<BaseEntity> OnEntityDeath = new();
    public static readonly UnityEvent OnWin = new(), OnLose = new();
     
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
    
    public void FinishPositionSetup()
    {
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
        
        var all = GridController.GetEntitiesOnGrid();
        _turnOrder = SetTurnOrder(all);
        NextTurn(true);
    }

    public void SetCombatStage(CombatState nxtStage)
    {
        currentStage = nxtStage;
        OnStagePass?.Invoke(currentStage);
    }
    
    private void SetCurrentEntity(BaseEntity entity)
    {
        GridManager.ClearGrid();

        CurrentTurn = new Turn(entity);

        SetCombatStage(TurnEntity.EntityInfo.entityType == EntityType.Playable
            ? CombatState.Playable
            : CombatState.Enemy);
        
        OnEntityTurn?.Invoke(TurnEntity);
        entity.StartTurn();
    }
    
    public static void SetMovementStage(BaseEntity entity)
    {
        if (entity != TurnEntity) return;
        
        CurrentTurn.SetTurnState(TurnState.Movement);
        
        TurnEntity.OnEntityMoved.AddListener(GridManager.ClearGrid);
        GridManager.ShowRadiusAsWalkable(TurnEntity);
    }
    public static void MovementAction(Cell cell)
    {
        TurnEntity.OnEntityMoved.AddListener(() => ActionTaken?.Invoke());
        TurnEntity.MoveTowards(cell);
    }

    public static void SetAttackStage(Skill skill)
    {
        if (CurrentTurn.UsedSkill) return;
        
        CurrentTurn.SetTurnState(TurnState.Attack);
        CurrentTurn.AssignSkill(skill);
        
        GridManager.ShowRadiusAsRange(TurnEntity, TurnSkill);
    }
    public static void AttackAction(Cell cell)
    {
        TurnSkill.OnSkillComplete.AddListener(delegate
        {
            CurrentTurn.UsedSkill = true;
            ActionTaken?.Invoke(); 
        });
        TurnSkill.OnSkillUse?.Invoke(cell);
    }
    
    #endregion
    
    private void Awake()
    { 
        Instance = this;
        CurrentTurn = null;
        currentStage = CombatState.Setup;
        
        GridManager.GridGenerated.AddListener(PositionEnemies);
        ActionTaken.AddListener(CheckActionsAvailable);
        OnEntityDeath.AddListener(CheckCombatState);
    }

    private void PositionEnemies()
    {
        LoadCombatSettings(GameManager.currentCombatInfo);
        
        foreach (var e in cSettings.enemieList)
        {
            var instance = e.enemie.GenerateEntity() as HostileEntity;

            var coord = GridController.GetCellAt(e.enemieCoord);     
            if (coord._entityInCell is not null)
                Debug.Log("Cell already occupied");
            
            instance?.SetPosition(coord);
        }

        if (cSettings.hasUnlock)
        {
            // Check if player has unlock here
            
            var e = cSettings.playableUnlocked;
            var unlockInstace = e.enemie.GenerateEntity() as HostileEntity;
            
            var coord = GridController.GetCellAt(e.enemieCoord);     
            if (coord._entityInCell is not null)
                Debug.Log("Cell already occupied");
            
            unlockInstace?.SetPosition(coord);
        }
    }

    private void CheckActionsAvailable()
    {
        if (TurnEntity.currentMovement > 0)
        {
            Debug.Log($"Ainda consegue andar {TurnEntity.currentMovement}");
            return;
        }
        if (!CurrentTurn.UsedSkill)
        {
            Debug.Log("Ainda pode usar skill");
            return;
        }
        
        NextTurn();
    }

    private void CheckCombatState(BaseEntity entity)
    {
        _turnOrder.Remove(entity);
        int playableCount = 0, enemieCount = 0;
        foreach (var ent in _turnOrder)
        {
            if (ent.EntityInfo.entityType == EntityType.Playable) 
                playableCount++;
            else if (ent.EntityInfo.entityType == EntityType.Hostile)
                enemieCount++;
        }

        if (enemieCount == 0)
        {
            GameManager.UpdateCurrency(10); // TODO Update currency based on a variable
            OnWin?.Invoke();
        }
        else if (playableCount == 0) OnLose?.Invoke();
    }
    
    private void ClearSelectedEntity(Cell selectedCell)
    {
        if (TurnEntity is null) return;
        if (TurnSkill is not null) CurrentTurn.AssignSkill(null);
            
        CurrentTurn.SetTurnState(TurnState.Neutral);
        GridManager.ClearGrid();
    }
}
