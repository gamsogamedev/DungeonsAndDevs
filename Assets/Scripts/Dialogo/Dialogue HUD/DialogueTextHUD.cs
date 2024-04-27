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
        DialogueManager.OnDialogueEvent.AddListener(WriteText);
        dialogueTextName.text = "";
        dialogueTextBox.text = "";
    }

    private void WriteText(MyDialogueInfo textToWrite)
    {
        dialogueTextName.text = textToWrite.whosTalking;
        dialogueTextBox.text = textToWrite.textLine;
    }

    public void CallNextDialogue()
    {
        // Debug.Log("NEXT");
        DialogueManager.OnNextDialogue?.Invoke();
    }
}
