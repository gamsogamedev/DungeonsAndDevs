using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/New Skill", fileName = "New Skill")]
public class ScriptableSkill : ScriptableObject
{
    [SerializeField] private string skillName;
    [SerializeField] private Sprite skillIcon;
    
    // TODO ADD -- Cost, Cooldown, etc...
    
    [Space(20)]
    [SerializeField] private List<CombatAction> skillActions;
}
