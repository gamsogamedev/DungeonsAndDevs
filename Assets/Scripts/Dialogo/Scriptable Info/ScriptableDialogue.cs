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

    [FormerlySerializedAs("animationPlaceholder")]
    [ShowIf("type", DialogueEventType.Animation)] [AllowNesting]
    public MyAnimationInfo animationInfo;

    [FormerlySerializedAs("choicePlaceholder")]
    [ShowIf("type", DialogueEventType.Choice)] [AllowNesting]
    public MyChoiceInfo choiceInfo;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class ScriptableDialogue : ScriptableObject
{
    public List<DialogueEvent> dialogueSequence;
}
