using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class ChoiceInfoEvent : UnityEvent<MyChoiceInfo>{}

[System.Serializable]
public class MyChoiceInfo
{
    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public int jumpToIndex;
        
        [Space(5)] 
        public bool updatesWorld;
        [ShowIf(nameof(updatesWorld)), AllowNesting] public WorldUpdate update;
    }

    public string choiceDescription;
    
    public Choice choiceA;
    public Choice choiceB;
    public Choice choiceC;
    public Choice choiceD;
}

