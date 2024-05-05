using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;

public class DialogueAnimationHUD : MonoBehaviour
{
    private PlayableDirector timelineManager;
    
    void Start()
    {
        timelineManager = GetComponent<PlayableDirector>();
        DialogueManager.OnAnimationEvent.AddListener(PlayAnimation);
    }

    private void PlayAnimation(MyAnimationInfo animationInfo)
    {
        timelineManager.playableAsset = animationInfo.cutscene;
        timelineManager.Play();
    }
    
}
