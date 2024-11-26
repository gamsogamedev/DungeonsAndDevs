using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TurnHighlight : MonoBehaviour
{
    public SpriteRenderer highlightCell;
    public SpriteRenderer highlightArrow;

    private Sequence highlightAnimation;
    private float baseArrowPosition;
    
    private void Awake()
    {
        SetHighlight(false);
        baseArrowPosition = highlightArrow.transform.localPosition.y;
    }

    public void HighlightEntity()
    {
        CombatManager.OnEntityTurn.AddListener(CancelHighlight);
        highlightAnimation = DOTween.Sequence();

        highlightAnimation.AppendCallback(delegate
        {
            highlightCell.color -= Color.black;
            highlightArrow.color -= Color.black;
            
            SetHighlight(true);
            Debug.Log(highlightArrow.enabled);
        });
        highlightAnimation.Append(highlightCell.DOFade(1f, .2f));
        highlightAnimation.Join(highlightArrow.DOFade(1f, .2f));

        highlightAnimation.Append(highlightArrow.transform.DOMoveY(highlightArrow.transform.position.y + .5f, 1f)
            .SetLoops(2, LoopType.Yoyo));
        highlightAnimation.AppendInterval(.5f);
        
        highlightAnimation.SetLoops(2, LoopType.Yoyo);
        highlightAnimation.OnComplete(() => SetHighlight(false));
    }

    private void CancelHighlight(BaseEntity entity)
    {
        CombatManager.OnEntityTurn.RemoveListener(CancelHighlight);
        highlightAnimation.Kill();

        var arrowTransform = highlightArrow.transform;
        arrowTransform.localPosition = new Vector3(arrowTransform.localPosition.x, baseArrowPosition, arrowTransform.localPosition.z);
        SetHighlight(false);
    }
    
    private void SetHighlight(bool state)
    {
        highlightCell.enabled = state;
        highlightArrow.enabled = state;
    }
}
