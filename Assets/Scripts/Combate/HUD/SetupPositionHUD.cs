using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SetupPositionHUD : MonoBehaviour
{
    private Canvas setupUI; // TODO: Change to CanvasGroup and animate a fade
    [SerializeField] private Button finishSetupButton;

    private void Start()
    {
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
        setupUI.enabled = false;
        CombatManager.Instance.FinishPositionSetup();
    }
}
