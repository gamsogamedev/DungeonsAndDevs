using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogueChoiceHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI choiceDescription;
    [SerializeField] private Button[] choice = new Button[4];
    private TextMeshProUGUI[] buttonName;
    
    private void Start()
    {
        DialogueManager.OnChoiceEvent.AddListener(SetupChoices);

        buttonName = new TextMeshProUGUI[choice.Length];
        for (var i = 0; i < choice.Length; i++)
        {
            buttonName[i] = choice[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void SetupChoices(MyChoiceInfo choiceInfo)
    {
        choiceDescription.text = choiceInfo.choiceDescription;
        
        buttonName[0].text = choiceInfo.choiceA.choiceText;
        choice[0].onClick.AddListener(() => CallJump(choiceInfo.choiceA));
        
        buttonName[1].text = choiceInfo.choiceB.choiceText;
        choice[1].onClick.AddListener(() => CallJump(choiceInfo.choiceB));
        
        buttonName[2].text = choiceInfo.choiceC.choiceText;
        choice[2].onClick.AddListener(() => CallJump(choiceInfo.choiceC));
        
        buttonName[3].text = choiceInfo.choiceD.choiceText;
        choice[3].onClick.AddListener(() => CallJump(choiceInfo.choiceD));
    }

    private void CallJump(MyChoiceInfo.Choice choice)
    {
        DialogueManager.LoadDialogue(choice.jumpToIndex);
    }
}
