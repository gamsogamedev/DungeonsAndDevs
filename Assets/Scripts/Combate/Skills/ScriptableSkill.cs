using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/New Skill", fileName = "New Skill")]
public class ScriptableSkill : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private Sprite skillIcon;
    
    // TODO ADD -- Cost, Cooldown, etc...
    
    [Space(20)]
    [SerializeField] private Range skillRange;
    [SerializeField] private List<CombatAction> skillActions;
    private Queue<CombatAction> actionsToDo;
    
    public void CastSkill(ITarget target)
    {
        actionsToDo = new Queue<CombatAction>();
        foreach (var action in skillActions)
        {
            actionsToDo.Enqueue(action);
        }
        
        ICombatAction.StepComplete.AddListener(ProccessAction);
        ProccessAction();
    }

    private void ProccessAction()
    {
        var currentAction = actionsToDo.Dequeue();
        currentAction.ExecuteAction();
    }
}
