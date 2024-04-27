using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    [SerializeField] private List<ScriptableDialogue> dialogues;
    private Queue<DialogueEvent> dialogueQueue;

    public static readonly UnityEvent<int> OnStartDialogue = new();
    public static readonly UnityEvent OnFinishDialogue = new();
    
    public static readonly UnityEvent OnNextDialogue = new();
    
    public static readonly DialogueInfoEvent OnDialogueEvent = new();
    public static readonly AnimationInfoEvent OnAnimationEvent = new();
    public static readonly ChoiceInfoEvent OnChoiceEvent = new();
    
    [SerializeField] private GameObject dialogueHUD,choiceHUD;
    
    private void Awake()
    {
        instance ??= this;
    }

    private void Start()
    {
        OnStartDialogue.AddListener(LoadDialogue);
        OnNextDialogue.AddListener(ProcessDialogue);
        
        dialogueHUD.SetActive(false);
        choiceHUD.SetActive(false);
        
        OnStartDialogue?.Invoke(0);
    }
    
    public void LoadDialogue(int startingIndex)
    {
        var curDialogue = dialogues[0];
        var curBlock = curDialogue.dialogueBlocks[startingIndex];
        
        dialogueQueue = new();
        foreach (var dio in curBlock.dialogueBlock) {
            dialogueQueue.Enqueue(dio);
            if (dio.type == DialogueEvent.DialogueEventType.Choice) break;
        }
        
        ProcessDialogue();
    }    

    private void ProcessDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            dialogueHUD.SetActive(false);
            choiceHUD.SetActive(false);
            OnFinishDialogue?.Invoke();
            return;
        }
        
        var dialogueEvent = dialogueQueue.Dequeue();
        switch (dialogueEvent.type)
        {
            case DialogueEvent.DialogueEventType.Dialogue:
                choiceHUD.SetActive(false);
                dialogueHUD.SetActive(true);
                Debug.Log(dialogueEvent.textLine.textLine);
                OnDialogueEvent?.Invoke(dialogueEvent.textLine);
                break;
            case DialogueEvent.DialogueEventType.Animation:
                OnAnimationEvent?.Invoke(dialogueEvent.animationInfo);
                break;
            case DialogueEvent.DialogueEventType.Choice:
                dialogueHUD.SetActive(false);
                choiceHUD.SetActive(true);
                OnChoiceEvent?.Invoke(dialogueEvent.choiceInfo);
                break;
            default:
                Debug.Log("dialogueEvent not recognized");
                break;
        }
    }
}
