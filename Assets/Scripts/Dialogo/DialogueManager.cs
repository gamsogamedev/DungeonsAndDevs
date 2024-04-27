using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    
    [SerializeField] private List<ScriptableDialogue> dialogues;
    private Queue<DialogueEvent> dialogueQueue;

    public static readonly UnityEvent OnStartDialogue = new(), OnFinishDialogue = new();
    public static readonly UnityEvent OnNextDialogue = new();
    public static readonly DialogueInfoEvent OnDialogueEvent = new();
    public static readonly AnimationInfoEvent OnAnimationEvent = new();
    public static readonly ChoiceInfoEvent OnChoiceEvent = new();
    
    private void Awake()
    {
        instance ??= this;
    }

    private void Start()
    {
        dialogueQueue = new();
        var dialogue = dialogues[0];
        foreach (var dio in dialogue.dialogueSequence) {
            dialogueQueue.Enqueue(dio);
            if (dio.type == DialogueEvent.DialogueEventType.Choice) break;
        }

        OnNextDialogue.AddListener(ProcessDialogue);
        ProcessDialogue();
    }

    public static void LoadDialogue(int startingIndex)
    {
        Debug.Log("Jumping to " + startingIndex);
    }    

    private void ProcessDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            //TODO Lógica de fechar o prompt de diálogo
            return;
        }
        var dialogueEvent = dialogueQueue.Dequeue();

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
