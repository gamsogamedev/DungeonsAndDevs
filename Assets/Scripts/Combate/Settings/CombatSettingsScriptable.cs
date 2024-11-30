using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Space(10)]

    public int mapLevel;
    
    [Space(10)]
    
    public bool hasUnlock;
    [ShowIf(nameof(hasUnlock))] public EnemieMapping playableUnlocked;
    [ShowIf(nameof(hasUnlock))] public EnemieMapping  SubstituteHostile;
    
    [Space(10)] 
    
    public bool hasDialogue;

    [ShowIf(nameof(hasDialogue)), SerializeField]
    private List<ScriptableDialogue> dialogueList;

    [ShowIf(EConditionOperator.And, nameof(hasDialogue), nameof(hasUnlock))]
    public bool hasUnlockableDialogue;
    [ShowIf(nameof(hasUnlockableDialogue)), SerializeField]
    private List<ScriptableDialogue> unlockableDialogueList;
    
    public ScriptableDialogue GetDialogue(bool isUnlocked)
    {
        if (!hasDialogue) return null;

        if (isUnlocked)
        {
            var randDialogue = Random.Range(0, dialogueList.Count);
            return dialogueList[randDialogue];
        }
        else
        {
            if (hasUnlockableDialogue)
            {
                var randDialogue = Random.Range(0, unlockableDialogueList.Count);
                return unlockableDialogueList[randDialogue];
            }
            else
            {
                var randDialogue = Random.Range(0, dialogueList.Count);
                return dialogueList[randDialogue];
            }
        }
    }
}
