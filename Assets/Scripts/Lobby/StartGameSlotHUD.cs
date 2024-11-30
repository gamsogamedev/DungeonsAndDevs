using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartGameSlotHUD : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image blocked;

    [SerializeField] private bool isPartySlot;
    public bool partySlot => isPartySlot;

    [SerializeField, HideIf(nameof(isPartySlot))]
    private ScriptableEntity_Playable entity;
    public ScriptableEntity_Playable GetEntity() => entity;
    public void SetEntity(ScriptableEntity_Playable e) => entity = e;
    
    private Button interactionBtn;

    public static readonly UnityEvent<StartGameSlotHUD> EntitySelected = new();
    
    private void Awake()
    {
        if (!isPartySlot)
        {
            SetSlotInfo(entity);

            if (GameManager.Instance.party.Contains(entity)) 
                MarkAsSelected(true);

            if (entity?.entityName == "Jogador") return;
        }

        interactionBtn = GetComponent<Button>();
        interactionBtn.onClick.AddListener(() => EntitySelected?.Invoke(this));
    }
    
    public void SetSlotInfo(ScriptableEntity_Playable e, bool empty = false)
    {
        if (e is null)
        {
            blocked.enabled = true;
            return;
        }

        entity = e;
        icon.sprite = e.entityVisuals;
        
        if (empty)
            icon.color = Color.black;
        else 
            icon.color = Color.white;
        
        if (GameManager.GetUnlock(e.entityName)) return;
        blocked.enabled = true;
    }

    public void MarkAsSelected(bool onParty)
    {
        icon.color = onParty ? new Color(.2f, .2f, .2f) : Color.white;
    }
}
