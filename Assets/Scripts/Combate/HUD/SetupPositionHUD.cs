using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SetupPositionHUD : MonoBehaviour
{
    private Canvas setupUI; // TODO: Change to CanvasGroup and animate a fade

    //SerializeField] private ButtonInstantiator Playable1;
    //[SerializeField] private ButtonInstantiator Playable2;
    //[SerializeField] private ButtonInstantiator Playable3;
    //[SerializeField] private ButtonInstantiator Playable4;
    
    [SerializeField] private List<ButtonInstantiator> PlayableList;

    [Space(20)]


    [SerializeField] private Button finishSetupButton;

    private void Start()
    {
        for (int i = 0; i < GameManager.Instance.party.Count; i++)
        {
            PlayableList[i].AssignEntity(GameManager.Instance.party[i]);
        }
        
        if (finishSetupButton is null)
        {
            Debug.LogError("Adicione uma referência ao botão [finishSetupButton]");
            return;
        }

        setupUI = GetComponent<Canvas>();
        setupUI.enabled = true;

        finishSetupButton.onClick.AddListener(StartCombat);

    }

    private void StartCombat()
    {
        for (int i = 0; i < GameManager.Instance.party.Count; i++){
            if (!PlayableList[i].valid) return;
        }
        
        setupUI.enabled = false; 
        CombatManager.Instance.FinishPositionSetup();
    }
}
