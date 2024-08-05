using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public enum ActionType
{
    None,
    Damage
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
        }
    }
    [AllowNesting, OnValueChanged(nameof(EditAction))] public ActionType actionType = ActionType.None;
    [SerializeReference] public ICombatAction action;

    public void ExecuteAction() => action.ExecuteAction();
}

[System.Serializable]
public class ICombatAction
{
    public static readonly UnityEvent StepComplete = new UnityEvent();

    public virtual void ExecuteAction()
    {
        StepComplete?.Invoke();
    }
}
