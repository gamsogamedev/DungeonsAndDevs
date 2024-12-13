using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderHUD : MonoBehaviour
{
    [SerializeField] private List<TurnOrderSlotHUD> turnOrderSlots;
    [SerializeField] private Image turnHighlight;
    
    private void Start()
    {
        foreach (var slot in turnOrderSlots)
        {
            slot.gameObject.SetActive(false);
        }

        turnHighlight.color -= Color.black;
        CombatManager.OnTurnOrderUpdate.AddListener(UpdateTurnOrderHUD);
        CombatManager.OnEntityTurn.AddListener(ActivateHighlight);
    }

    private void ActivateHighlight(BaseEntity entity)
    {
        var seq = DOTween.Sequence();

        seq.Append(turnHighlight.DOFade(0f, .5f));
        seq.AppendCallback(() => ShiftHighlight(entity));
        seq.Append(turnHighlight.DOFade(1f, .5f));
    }

    private void ShiftHighlight(BaseEntity entity)
    {
        // TODO: Make it handle this using the entity itself instead of the scriptable (bad bad bad)
        var slot = turnOrderSlots.First(slot => slot.GetEntity() == entity);
        turnHighlight.transform.SetParent(slot.transform);
        turnHighlight.transform.SetAsFirstSibling();
        turnHighlight.transform.localPosition = Vector3.zero;
    }

    private void UpdateTurnOrderHUD(List<BaseEntity> turnOrder)
    {
        foreach (var slot in turnOrderSlots)
        {
            slot.gameObject.SetActive(false);
        }
        
        var i = 0;
        foreach (var ent in turnOrder)
        {
            turnOrderSlots[i].SetSlotInfo(ent);
            turnOrderSlots[i].gameObject.SetActive(true);
            i++;
        }
    }
}
