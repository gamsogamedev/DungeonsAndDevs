using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;using UnityEngine.Events;

public class AnimationInfoEvent : UnityEvent<MyAnimationInfo> {}

[System.Serializable]
public class MyAnimationInfo
{
    public enum AnimationType {MOVE, TURN, EMOTE}

    [System.Serializable]
    public class Anim
    {
        [AllowNesting] public AnimationType animationType;
        [AllowNesting] public bool needDirection, needDistance;

        [AllowNesting] [ShowIf("needDirection")] public int direction;
        [AllowNesting] [ShowIf("needDistance")] public int distance;
    }
    
    public List<Anim> animationSequence;
}
