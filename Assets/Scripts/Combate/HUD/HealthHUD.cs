using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    [SerializeField] private Image healthbarUI;
    
    public void Init(BaseEntity hudOwner)
    {
        if (TryGetComponent<Canvas>(out var wCanva)) 
            wCanva.worldCamera = Camera.main;    
        
        hudOwner.HealthUpdated.AddListener(UpdateHUD);
        healthbarUI.fillAmount = 1;

        Debug.Log($"{hudOwner.gameObject.name}'s health ui set");
    }

    private void UpdateHUD(float healthPercentage)
    {
        healthbarUI.DOFillAmount(healthPercentage, .5f);
    }
}
