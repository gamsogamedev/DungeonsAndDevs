using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class DialogueEvent
{
    public enum DialogueEventType { Dialogue, Choice, Animation };

    public DialogueEventType type;

    [ShowIf("type", DialogueEventType.Dialogue)] [AllowNesting]
    public MyDialogueInfo textLine;
    
    [ShowIf("type", DialogueEventType.Animation)] [AllowNesting]
    public MyAnimationInfo animationInfo;
    
    [ShowIf("type", DialogueEventType.Choice)] [AllowNesting]
    public MyChoiceInfo choiceInfo;
}

[System.Serializable]
public class DialogueBlock
{
    [AllowNesting] public List<DialogueEvent> dialogueBlock;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class ScriptableDialogue : ScriptableObject
{
    public List<DialogueBlock> dialogueBlocks;
}
