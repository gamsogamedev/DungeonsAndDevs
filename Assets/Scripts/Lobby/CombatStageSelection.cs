using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatStageSelection : MonoBehaviour
{
    [SerializeField] private CanvasGroup confirmOverlay;
    [SerializeField] private Button confirmButton, rejectButton;

    [Serializable]
    public class MapButton
    {
        public Button btn;
        public CombatSettingsScriptable combatSettings;
    }

    [SerializeField] private List<MapButton> combatTrail;
    private CombatSettingsScriptable selectedSettings;
    
    private void Awake()
    {
        confirmOverlay.alpha = 0;
        confirmOverlay.interactable = false;
        confirmOverlay.blocksRaycasts = false;

        confirmButton.onClick.AddListener(LoadNextCombat);
        rejectButton.onClick.AddListener(CloseOverlay);
        
        foreach (var comb in combatTrail)
        {

            if (comb.combatSettings.mapLevel == GameManager.Instance.GetMapProgress()){
                comb.btn.onClick.AddListener(delegate
                {
                    selectedSettings = comb.combatSettings;
                    OpenOverlay();
                });
            }
            else{
                if (comb.combatSettings.mapLevel < GameManager.Instance.GetMapProgress()){
                    // Adicionar um check de complete talvez?
                    comb.btn.image.color = Color.green;
                    comb.btn.interactable = false;
                }else{
                    // Adicionar aquele cadeadinho maneiro
                    comb.btn.image.color = Color.red;
                    comb.btn.interactable = false;
                }
            }

            
        }
    }

    private void OpenOverlay()
    {
        confirmButton.interactable = false; rejectButton.interactable = false;
        confirmOverlay.DOFade(1f, 1f)
            .OnComplete(delegate
            {
                confirmOverlay.interactable = true;
                confirmOverlay.blocksRaycasts = true;
                confirmButton.interactable = true; rejectButton.interactable = true;
            });
    }
    
    private void CloseOverlay()
    {
        confirmButton.interactable = false; rejectButton.interactable = false;
        confirmOverlay.DOFade(0f, 1f)
            .OnComplete(delegate
            {
                confirmOverlay.interactable = false;
                confirmOverlay.blocksRaycasts = false;
            });
    }

    private void LoadNextCombat()
    {
        GameManager.SetNewGame(selectedSettings);
        SceneManager.LoadScene("Cenas/TesteCombate");
    }
}
