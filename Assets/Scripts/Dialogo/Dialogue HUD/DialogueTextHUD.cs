using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTextHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueTextBox;
    [SerializeField] private TextMeshProUGUI dialogueTextName;
    private void Awake()
    {
        Debug.Log("BBBB");
        DialogueManager.OnDialogueEvent.AddListener(WriteText);
        DialogueManager.OnDialogueEvent.AddListener((MyDialogueInfo info) => Debug.Log("Event call"));
        DialogueManager.OnChoiceEvent.AddListener(Disable);
        DialogueManager.OnFinishDialogue.AddListener(() => gameObject.SetActive(false));
        
        dialogueTextName.text = "";
        dialogueTextBox.text = "";
        
        this.gameObject.SetActive(false);
    }

    private void Disable(MyChoiceInfo info) => this.gameObject.SetActive(false);

    private void WriteText(MyDialogueInfo textToWrite)
    {
        Debug.Log("AAA");
        this.gameObject.SetActive(true);
        
        dialogueTextName.text = textToWrite.whosTalking;
        dialogueTextBox.text = textToWrite.textLine;
    }

    public void CallNextDialogue()
    {
        // Debug.Log("NEXT");
        DialogueManager.OnNextDialogue?.Invoke();
    }
}
