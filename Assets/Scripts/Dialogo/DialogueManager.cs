using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private List<ScriptableDialogue> dialogues;
    private Queue<DialogueEvent> _dialogueQueue;

    public static readonly UnityEvent<int> OnStartDialogue = new();
    public static readonly UnityEvent OnFinishDialogue = new();
    
    public static readonly UnityEvent OnNextDialogue = new();
    
    public static readonly DialogueInfoEvent OnDialogueEvent = new();
    public static readonly AnimationInfoEvent OnAnimationEvent = new();
    public static readonly ChoiceInfoEvent OnChoiceEvent = new();

    private bool _canInteract;

    private void Start()
    {
        OnStartDialogue.AddListener(LoadDialogue);
        OnNextDialogue.AddListener(ProcessDialogue);
    }
    
    public void LoadDialogue(int startingIndex)
    {
        var curDialogue = dialogues[0];
        var curBlock = curDialogue.dialogueBlocks[startingIndex];
        
        _dialogueQueue = new();
        foreach (var dio in curBlock.dialogueBlock) {
            _dialogueQueue.Enqueue(dio);
            if (dio.type == DialogueEvent.DialogueEventType.Choice) break;
        }
        
        ProcessDialogue();
    }    

    private void ProcessDialogue()
    {
        if (_dialogueQueue.Count == 0)
        {
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        _canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        _canInteract = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _canInteract) {
            OnStartDialogue?.Invoke(0);
        }
    }
}
