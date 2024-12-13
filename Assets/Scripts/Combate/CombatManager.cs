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
    
    // ----- COMBAT SETTINGS
    [SerializeField] private CombatSettingsScriptable cSettings;
    public CombatSettingsScriptable GetSettings() => cSettings;
    private void SetCombatSettings(CombatSettingsScriptable sett) => cSettings = sett;
    
    // ----- TURN INFO
    public class Turn
    {
        public BaseEntity TurnSelectedEntity;
        public Skill TurnSelectedSkill;
        public bool UsedSkill;
        public TurnState CurrentState;

        public Turn(BaseEntity turnEntity)
        {
            this.TurnSelectedEntity = turnEntity;
            
            this.TurnSelectedSkill = null;
            this.UsedSkill = false;
            
            this.TurnSelectedEntity.ResetMovement();
            
            CurrentState = TurnState.Neutral;
        }

        public void SetTurnState(TurnState state) => CurrentState = state;

        public void AssignSkill(Skill skill)
        {
            TurnSelectedSkill = skill;
            TurnSelectedSkill?.SetupSkill();
        }
    }
    public static Turn CurrentTurn { get; private set; }
    public static BaseEntity TurnEntity => CurrentTurn.TurnSelectedEntity;
    public static Skill TurnSkill => CurrentTurn.TurnSelectedSkill;
    
    
    // ----- TURN EVENTS
    public static readonly UnityEvent<BaseEntity> OnEntityTurn = new(); 
    public static readonly UnityEvent ActionTaken = new();
    
    // ----- OTHER EVENTS
    public static readonly UnityEvent<BaseEntity> OnEntityDeath = new();
    public static readonly UnityEvent OnWin = new(), OnLose = new();
     
    #region Turn Order

    private List<BaseEntity> _turnOrder;

    public static readonly UnityEvent<List<BaseEntity>> OnTurnOrderUpdate = new();
    
    private List<BaseEntity> SetTurnOrder(List<BaseEntity> entitiesInvolved)
    {
        var orderList = entitiesInvolved.OrderBy(e => e.EntityInfo.GetInitiative());
        return orderList.ToList();
    }

    public void PassTurn()
    {
        Invoke(nameof(NextTurn), .5f);
    }
    
    public void NextTurn()
    {
        var entityIdx = _turnOrder.FindIndex(ent => ent == TurnEntity);
        var orderCount = (entityIdx + 1 >= _turnOrder.Count) ? 0 : entityIdx + 1;
        SetCurrentEntity(_turnOrder[orderCount]);
    }

    #endregion
    
    #region Stage Management

    public CombatState currentStage;
    public static readonly UnityEvent<CombatState> OnStagePass = new();

    private void SetCombatStage(CombatState nxtStage)
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
    
    // SETUP STAGE ------------------------------------------------------
    public void FinishPositionSetup()
    {
        GridManager.OnSelect.AddListener(ClearSelectedEntity);
        
        var all = GridController.GetEntitiesOnGrid();
        _turnOrder = SetTurnOrder(all);
        OnTurnOrderUpdate?.Invoke(_turnOrder);
        
        SetCurrentEntity(_turnOrder[0]);
    }
    
    // SETUP STAGE ------------------------------------------------------
    public static void SetMovementStage(BaseEntity entity)
    {
        if (entity != TurnEntity) return;
        
        CurrentTurn.SetTurnState(TurnState.Movement);
        
        GridManager.ShowRadiusAsWalkable(TurnEntity);
    }
    public static void MovementAction(Cell cell)
    {
        TurnEntity.OnEntityMoved.RemoveListener(MovementStageCleanup);
        TurnEntity.OnEntityMoved.AddListener(MovementStageCleanup);
        TurnEntity.MoveTowards(cell);
    }

    public static void MovementStageCleanup()
    {
        GridManager.ClearGrid();
        ActionTaken?.Invoke();
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
        //LoadCombatSettings(GameManager.currentCombatInfo);

        if (GameManager.currentCombatInfo is not null){
            SetCombatSettings(GameManager.currentCombatInfo);
        }
        
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
            var entName = cSettings.playableUnlocked.enemie.entityName;
            EnemieMapping unlockable;
            if (!GameManager.GetUnlock(entName))
            {
                unlockable = cSettings.playableUnlocked;
            }
            else
            {
                unlockable = cSettings.SubstituteHostile;
                
                //OnWin.AddListener(() => GameManager.SetUnlock(cSettings.playableUnlocked.enemie.entityName));
            }
            
            var unlockInstance = unlockable.enemie.GenerateEntity() as HostileEntity;
            var coord = GridController.GetCellAt(unlockable.enemieCoord);     
            if (coord._entityInCell is not null)
                Debug.Log("Cell already occupied");
            unlockInstance?.SetPosition(coord);
        }
        
        Invoke(nameof(ProccesDialogue), 2f);
    }

    private void ProccesDialogue()
    {
        ScriptableDialogue dialogue;
        if (!cSettings.hasDialogue) return;
        
        
        if (cSettings.hasUnlock)
        {
            var isUnlocked = GameManager.GetUnlock(cSettings.playableUnlocked.enemie.entityName);
            dialogue = cSettings.GetDialogue(isUnlocked);
        }
        else
        {
            dialogue = cSettings.GetDialogue(true);
        }

        // TODO block interaction during dialogue (Waiting for party system impl)
        DialogueManager.OnStartDialogue?.Invoke(dialogue);
    }

    private void CheckActionsAvailable()
    {
        if (TurnEntity.currentMovement > 0)
        {
            Debug.Log($"Can still walk {TurnEntity.currentMovement}");
            return;
        }
        if (!CurrentTurn.UsedSkill)
            return;
        
        PassTurn();
    }

    private void CheckCombatState(BaseEntity entity)
    {
        _turnOrder.Remove(entity);
        _turnOrder = SetTurnOrder(_turnOrder);
        OnTurnOrderUpdate?.Invoke(_turnOrder);
        
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
