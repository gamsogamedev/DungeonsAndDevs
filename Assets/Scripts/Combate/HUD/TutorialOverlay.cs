using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private Button okButton;

    private void Awake()
    {
        overlay.alpha = 0;
        overlay.interactable = false;
        overlay.blocksRaycasts = false;
        
        okButton.onClick.AddListener(CloseOverlay);

    }

    private void Start()
    {
        if (GameManager.GetUnlock("Tutorial")) 
        {
            this.gameObject.SetActive(false);
            return;
        }

        overlay.DOFade(1f, .5f)
            .OnComplete(delegate
            {
                overlay.interactable = true;
                overlay.blocksRaycasts = true;
            });
    }

    private void CloseOverlay()
    {
        GameManager.SetUnlock("Tutorial");
        
        overlay.DOFade(0f, 1f)
            .OnComplete(delegate
            {
                this.gameObject.SetActive(false);
            });
    }
}
