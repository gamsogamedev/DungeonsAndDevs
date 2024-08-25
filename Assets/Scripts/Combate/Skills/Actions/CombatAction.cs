using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Events;

public enum ActionType
{
    None,
    Damage,
    Heal,
    Move,
    Desloc
}

[System.Serializable]
public class CombatAction
{
    public string actionDescription;
    public void EditAction()
    {
        switch (actionType)
        {
            default:
            case ActionType.Damage:
                action = new DamageAction();
                break;
            case ActionType.Heal:
                action = new HealAction();
                break;
            case ActionType.Move:
                action = new MoveAction();
                break;
            case ActionType.Desloc:
                action = new DeslocAction();
                break;
        }
    }
    [AllowNesting, OnValueChanged(nameof(EditAction))] public ActionType actionType = ActionType.None;
    [SerializeReference] public ICombatAction action;
    
    // ----- EVENTOS
    public static readonly UnityEvent<Cell> OnActionComplete = new();
    
    public void ExecuteAction(BaseEntity caster, Cell target)
    {
        Cell nextTarget = action.ExecuteAction(caster, target);
        OnActionComplete?.Invoke(nextTarget);
    }
    public List<Cell> PreviewRange(Cell center) => action.PreviewRange(center);
}

public interface ICombatAction
{
    Cell ExecuteAction(BaseEntity caster, Cell target);
    List<Cell> PreviewRange(Cell center);
}
