using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueInfoEvent : UnityEvent<MyDialogueInfo>{}

[System.Serializable]
public class MyDialogueInfo
{
    [TextArea] public string textLine;
    
    public string whosTalking;
    public Sprite whosTalkingIcon;
}
