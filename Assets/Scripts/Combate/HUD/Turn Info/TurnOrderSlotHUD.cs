using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnOrderSlotHUD : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    private BaseEntity entity;
    public BaseEntity GetEntity() => entity;
    
    public void SetEntity(BaseEntity e) => entity = e;

    public void SetSlotInfo(BaseEntity e)
    {
        SetEntity(e);
        icon.sprite = e.EntityInfo.entityVisuals;
    }
}
