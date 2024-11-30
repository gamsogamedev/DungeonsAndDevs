using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Skill
{
    // ----- Skill Config
    public ScriptableSkill skillInfo;
    
    // ----- References used
    private Cell skillTarget;
    private PlayableEntity skillCaster;
    public Range SkillRange => skillInfo.skillRange;
    
    private Queue<CombatAction> actionsToDo;
    
    // ----- Eventos
    public readonly UnityEvent
        OnSkillComplete = new();
    public readonly UnityEvent<Cell>
        OnSkillUse = new();

    public Skill(ScriptableSkill skillConfig, PlayableEntity caster)
    {
        skillInfo = skillConfig;
        skillCaster = caster;
    }

    public void SetupSkill()
    {
        OnSkillUse.RemoveAllListeners();
        OnSkillUse.AddListener(CastSkill);
        
        OnSkillComplete.RemoveAllListeners();
        OnSkillComplete.AddListener(GridManager.ClearGrid);
    }
    
    /// <summary>
    /// Função chamada quando a skill é usada
    /// </summary>
    /// <param name="target"></param>
    public void CastSkill(Cell target)
    {
        if (target._entityInCell is not null)
        {
            if (target._entityInCell.EntityInfo.entityType == skillCaster.EntityInfo.entityType) return;
        }
        actionsToDo = new Queue<CombatAction>();
        foreach (var action in skillInfo.skillActions)
        {
            actionsToDo.Enqueue(action);
        }
        
        CombatAction.OnActionComplete.AddListener(UpdateTarget);
        UpdateTarget(target);
    }

    private void UpdateTarget(Cell newTarget)
    {
        skillTarget = newTarget;

        var skillDir = newTarget.cellCoord - skillCaster.currentCell.cellCoord;
        skillCaster.FixSort(skillDir);
        
        ProccessAction();
    }
    private void ProccessAction()
    {
        if (actionsToDo.Count <= 0)
        {
            CombatAction.OnActionComplete.RemoveAllListeners();
            OnSkillComplete?.Invoke();
            return;
        }
        
        var currentAction = actionsToDo.Dequeue();
        currentAction.ExecuteAction(skillCaster, skillTarget);
    }

    // Visual Info on the skill
    public List<Cell> PreviewSkill(Cell center)
    {
        return skillInfo.skillActions[0].PreviewRange(center);
    }
}
