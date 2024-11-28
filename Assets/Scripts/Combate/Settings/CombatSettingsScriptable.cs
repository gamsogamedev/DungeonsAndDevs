using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Combat Settings", fileName = "New Combat Settings")]
public class CombatSettingsScriptable : ScriptableObject
{
    public Vector2Int gridSize;
    
    public List<EnemieMapping> enemieList;
    
    public bool hasUnlock;
    [ShowIf(nameof(hasUnlock))] public ScriptableEntity_Playable playableUnlocked;
}
