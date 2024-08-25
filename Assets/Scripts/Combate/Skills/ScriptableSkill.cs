using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Adicionar builds -- Skill set
/// Adicionar selecao de skills
/// Coisas pro compilador não ser uma lista e mais uma gramática 
/// </summary>

[CreateAssetMenu(menuName = "Skill/New Skill", fileName = "New Skill")]
public class ScriptableSkill : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    
    // TODO ADD -- Cost, Cooldown, etc... 
    
    [Space(20)]
    public Range skillRange;
    public List<CombatAction> skillActions;
}
