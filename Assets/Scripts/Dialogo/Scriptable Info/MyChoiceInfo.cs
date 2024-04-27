using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    public string choiceDescription;
    
    public Choice choiceA;
    public Choice choiceB;
    public Choice choiceC;
    public Choice choiceD;
}

