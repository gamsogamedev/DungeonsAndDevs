using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DialogueInfoEvent : UnityEvent<MyDialogueInfo>{}

[System.Serializable]
public class MyDialogueInfo
{
    [ResizableTextArea] public string textLine;
    
    public string whosTalking;
    public Sprite whosTalkingIcon;
}
