using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class AnimationInfoEvent : UnityEvent<MyAnimationInfo> {}

[System.Serializable]
public class MyAnimationInfo
{
    [AllowNesting] public TimelineAsset cutscene;
}