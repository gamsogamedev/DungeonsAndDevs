using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceHUD : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup choiceGrid;
    private Vector2 slotDim;
    
    [SerializeField] private TextMeshProUGUI choiceDescription;
    
    [SerializeField] private Button[] choice = new Button[4];
    private TextMeshProUGUI[] buttonName;
    
    private void Awake()
    {
        DialogueManager.OnChoiceEvent.AddListener(SetupChoices);
        DialogueManager.OnDialogueEvent.AddListener(Disable);
        DialogueManager.OnAnimationEvent.AddListener(Disable);
        DialogueManager.OnFinishDialogue.AddListener(() => gameObject.SetActive(false));

        buttonName = new TextMeshProUGUI[choice.Length];
        for (var i = 0; i < choice.Length; i++)
        {
            buttonName[i] = choice[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        slotDim = choiceGrid.cellSize;
        this.gameObject.SetActive(false);
    }
    
    private void Disable(MyDialogueInfo info) => this.gameObject.SetActive(false);
    private void Disable(MyAnimationInfo info) => this.gameObject.SetActive(false);

    
    private void SetupChoices(MyChoiceInfo choiceInfo)
    {
        this.gameObject.SetActive(true);
        choiceDescription.text = choiceInfo.choiceDescription;

        var choiceA = choiceInfo.choiceA;
        if (choiceA.choiceText.Equals("")) {
            choice[0].gameObject.SetActive(false);
        }
        else {
            choice[0].gameObject.SetActive(true);
            buttonName[0].text = choiceA.choiceText;
            choice[0].onClick.AddListener(() => CallJump(choiceA));            
        }

        var choiceB = choiceInfo.choiceB;
        if (choiceB.choiceText.Equals("")) {
            choice[1].gameObject.SetActive(false);
        }
        else {
            choice[1].gameObject.SetActive(true);
            buttonName[1].text = choiceB.choiceText;
            choice[1].onClick.AddListener(() => CallJump(choiceB));            
        }
        
        var choiceC= choiceInfo.choiceC;
        if (choiceC.choiceText.Equals(""))
        {
            choiceGrid.cellSize = new Vector2(slotDim.x, 2 * slotDim.y);
            choice[2].gameObject.SetActive(false);
        }
        else {
            choiceGrid.cellSize = new Vector2(slotDim.x, slotDim.y);
            choice[2].gameObject.SetActive(true);
            buttonName[2].text = choiceC.choiceText;
            choice[2].onClick.AddListener(() => CallJump(choiceC));            
        }
        
        var choiceD = choiceInfo.choiceD;
        if (choiceD.choiceText.Equals("")) {
            choice[3].gameObject.SetActive(false);
        }
        else {
            choice[3].gameObject.SetActive(true);
            buttonName[3].text = choiceD.choiceText;
            choice[3].onClick.AddListener(() => CallJump(choiceD));            
        }
    }

    private void CallJump(MyChoiceInfo.Choice buttonChoice)
    {
        if (buttonChoice.updatesWorld)
        {
            GameManager.SetUnlock(buttonChoice.update.nodeName);
        }
        
        DialogueManager.OnNextDialogueBlock?.Invoke(buttonChoice.jumpToIndex);
    }
}
