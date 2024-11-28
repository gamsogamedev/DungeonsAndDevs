using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class EnemieMapping
{
    public ScriptableEntity_Hostile enemie;
    public Vector2Int enemieCoord;
}

[CreateAssetMenu(menuName = "Settings/Combat Settings", fileName = "New Combat Settings")]
public class CombatSettingsScriptable : ScriptableObject
{
    public Vector2Int gridSize;
    
    public List<EnemieMapping> enemieList;
    
    public bool hasUnlock;
    [ShowIf(nameof(hasUnlock))] public EnemieMapping playableUnlocked;
    [ShowIf(nameof(hasUnlock))] public EnemieMapping  SubstituteHostile;
}
