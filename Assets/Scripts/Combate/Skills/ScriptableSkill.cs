using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Skill/New Skill", fileName = "New Skill")]
public class ScriptableSkill : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    
    // TODO ADD -- Cost, Cooldown, etc...
    
    [Space(20)]
    [SerializeField] private List<CombatAction> skillActions;
    private Queue<CombatAction> actionsToDo;
    
    public Range skillRange;

    public int cooldownTime; // EM TURNOS?

    private ITarget skillTarget;
    
    public readonly UnityEvent
        OnSkillSelected = new(),
        OnSkillComplete = new();
    public readonly UnityEvent<Cell>
        OnSkillUse = new();
    
    public void CastSkill(ITarget target)
    {
        skillTarget = target;
            
        actionsToDo = new Queue<CombatAction>();
        foreach (var action in skillActions)
        {
            actionsToDo.Enqueue(action);
        }
        
        ICombatAction.OnActionComplete.AddListener(ProccessAction);
        ProccessAction();
    }

    private void ProccessAction()
    {
        if (actionsToDo.Count <= 0)
        {
            OnSkillComplete?.Invoke();
            return;
        }
        
        
        var currentAction = actionsToDo.Dequeue();
        
        currentAction.ExecuteAction(skillTarget);
    }

    public List<Cell> PreviewSkill(Cell center)
    {
        return skillActions[0].PreviewRange(center);
    }
}
