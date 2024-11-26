using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndCombatOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup victoryOverlay;
    [SerializeField] private Button continueBtn;
    [Space(5)]
    [SerializeField] private CanvasGroup defeatOverlay;
    [SerializeField] private Button homeBtn;

    private void Awake()
    {
        CombatManager.OnWin.AddListener(OpenWinOverlay);
        ControlOverlay(victoryOverlay, false);
        // continueBtn.onClick.AddListener();
        
        CombatManager.OnLose.AddListener(OpenDefeatOverlay);
        ControlOverlay(defeatOverlay, false);
        // homeBtn.onClick.AddListener();
    }

    private void ControlOverlay(CanvasGroup overlay, bool activate)
    {
        if (activate)
        {
            overlay.DOFade(1f, .5f)
                .OnComplete(delegate { overlay.interactable = true; overlay.blocksRaycasts = true; });
        }
        else
        {
            overlay.alpha = 0;
            overlay.interactable = false;
            overlay.blocksRaycasts = false;
        }
    }
    
    private void OpenWinOverlay()
    {
        ControlOverlay(victoryOverlay, true);
    }
    
    private void OpenDefeatOverlay()
    {
        ControlOverlay(defeatOverlay, true);
    }
}
