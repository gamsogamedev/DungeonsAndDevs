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
        GetComponent<Canvas>().worldCamera = Camera.main;
        
        hudOwner.HealthUpdated.AddListener(UpdateHUD);
        healthbarUI.fillAmount = 1;
    }

    private void UpdateHUD(float healthPercentage)
    {
        Debug.Log($"Updating life to {healthPercentage}");
        healthbarUI.DOFillAmount(healthPercentage, .5f);
    }
}
