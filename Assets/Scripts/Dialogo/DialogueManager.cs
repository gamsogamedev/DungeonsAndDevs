using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private Queue<DialogueEvent> _dialogueQueue;

    public static readonly UnityEvent<ScriptableDialogue> OnStartDialogue = new ();
    public static readonly UnityEvent<int> OnNextDialogueBlock = new();
    public static readonly UnityEvent OnFinishDialogue = new();
    
    public static readonly UnityEvent OnNextDialogue = new();
    
    public static readonly DialogueInfoEvent OnDialogueEvent = new();
    public static readonly AnimationInfoEvent OnAnimationEvent = new();
    public static readonly ChoiceInfoEvent OnChoiceEvent = new();

    private ScriptableDialogue _currentDialogue;
    private DialogueBlock _currentDialogueBlock;

    private void Awake()
    {
        instance ??= this;
    }

    private void Start()
    {
        OnStartDialogue.AddListener(InitDialogue);
        OnNextDialogue.AddListener(ProcessDialogue);
        OnNextDialogueBlock.AddListener(LoadBlock);
    }

    private void InitDialogue(ScriptableDialogue dialogueToParse)
    {
        _currentDialogue = dialogueToParse;
        OnNextDialogueBlock?.Invoke(0);
    }
    
    public void LoadBlock(int blockIdx)
    {
        _currentDialogueBlock = _currentDialogue.dialogueBlocks[blockIdx];
        
        _dialogueQueue = new();
        foreach (var dio in _currentDialogueBlock.dialogueBlock) {
            _dialogueQueue.Enqueue(dio);
            if (dio.type == DialogueEvent.DialogueEventType.Choice) break;
        }

        if (_currentDialogueBlock.updatesWorld)
        { 
            GameManager.UpdateWorldState?.Invoke(_currentDialogueBlock.update);
        }
        
        ProcessDialogue();
    }    

    private void ProcessDialogue()
    {
        if (_dialogueQueue.Count == 0)
        {
            if (_currentDialogueBlock.overrideJump)
            {
                OnNextDialogueBlock?.Invoke(_currentDialogueBlock.jumpToBlock);
            }
            
            OnFinishDialogue?.Invoke();
            return;
        }
        
        var dialogueEvent = _dialogueQueue.Dequeue();
        switch (dialogueEvent.type)
        {
            case DialogueEvent.DialogueEventType.Dialogue:
                OnDialogueEvent?.Invoke(dialogueEvent.textLine);
                break;
            case DialogueEvent.DialogueEventType.Animation:
                OnAnimationEvent?.Invoke(dialogueEvent.animationInfo);
                break;
            case DialogueEvent.DialogueEventType.Choice:
                OnChoiceEvent?.Invoke(dialogueEvent.choiceInfo);
                break;
            default:
                Debug.Log("dialogueEvent not recognized");
                break;
        }
    }
}
