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
        OnSkillSelected = new(),
        OnSkillComplete = new();
    public readonly UnityEvent<Cell>
        OnSkillUse = new();

    public Skill(ScriptableSkill skillConfig, PlayableEntity caster)
    {
        skillInfo = skillConfig;
        skillCaster = caster;
    }
    
    /// <summary>
    /// Função chamada quando a skill é usada
    /// </summary>
    /// <param name="target"></param>
    public void CastSkill(Cell target)
    {
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
        ProccessAction();
    }

    /// <summary>
    /// Função chamada por SkillCast para chamar uma unidade de ação
    /// </summary>
    private void ProccessAction()
    {
        if (actionsToDo.Count <= 0)
        {
            OnSkillComplete?.Invoke();
            return;
        }
        
        var currentAction = actionsToDo.Dequeue();
        currentAction.ExecuteAction(skillCaster, skillTarget);
    }

    public List<Cell> PreviewSkill(Cell center)
    {
        //return new List<Cell>();
        return skillInfo.skillActions[0].PreviewRange(center);
    }
}
