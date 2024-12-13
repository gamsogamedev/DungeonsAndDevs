using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EntitySlotHUD : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image blocked;

    public bool interactable;
    public bool isPartySlot;

    [SerializeField, HideIf(nameof(isPartySlot))]
    private ScriptableEntity entity;
    public ScriptableEntity GetEntity() => entity;
    public ScriptableEntity_Playable GetPlayableEntity() => entity as ScriptableEntity_Playable;
    public void SetEntity(ScriptableEntity e) => entity = e;
    
    private Button interactionBtn;

    public static readonly UnityEvent<EntitySlotHUD> EntitySelected = new();
    
    private void Awake()
    {
        if (!isPartySlot && interactable)
        {
            SetSlotInfo(entity);

            if (GameManager.Instance.party.Contains(entity as ScriptableEntity_Playable)) 
                MarkAsSelected(true);
        }

        if (interactable)
        {
            interactionBtn = GetComponent<Button>();
            interactionBtn.onClick.AddListener(() => EntitySelected?.Invoke(this));
        }
    }
    
    public void SetSlotInfo(ScriptableEntity e, bool empty = false)
    {
        if (e is null)
        {
            blocked.enabled = true;
            return;
        }

        SetEntity(e);
        icon.sprite = e.entityVisuals;
        
        if (empty)
            icon.color = Color.black;
        else 
            icon.color = Color.white;
        
        if (GameManager.GetUnlock(e.entityName)) return;
        if (!interactable) return;

        blocked.enabled = true;
    }

    public void MarkAsSelected(bool onParty)
    {
        icon.color = onParty ? new Color(.2f, .2f, .2f) : Color.white;
    }
}
