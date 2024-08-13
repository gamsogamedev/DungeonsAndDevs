using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PlayableHUD : MonoBehaviour
{
    [System.Serializable]
    public class SkillVisual
    {
        public Button button;
        public Image icon;
        public Image cooldownOverlay;

        public void SetupVisual(ScriptableSkill skill)
        {
            icon.sprite = skill.skillIcon;
            cooldownOverlay.fillAmount = 0;
            
            button.onClick.AddListener(() => skill.OnSkillSelected?.Invoke());
        }
    }
    
    [SerializeField] private SkillVisual skill1;
    [SerializeField] private SkillVisual skill2;
    [SerializeField] private SkillVisual skill3;
    [SerializeField] private SkillVisual skill4;

    private void Start()
    {
        CombatManager.OnEntitySelected.AddListener(UpdateHUD);
    }

    private void UpdateHUD(BaseEntity entity)
    {
        // TEMPORARY !!!
        if (CombatManager.Instance.currentStage != CombatState.PlayerAttack) return;
        
        var playableEntity = entity.EntityInfo.ToPlayable();
        if (playableEntity.entityType != EntityType.Playable)
        {
            HideUI();
            return;
        }
        
        skill1.SetupVisual(playableEntity.skill1);
        skill2.SetupVisual(playableEntity.skill2);
        skill3.SetupVisual(playableEntity.skill3);
        skill4.SetupVisual(playableEntity.skill4);
        ShowUI();
    }

    private void ShowUI()
    {
        var playableHUD = GetComponent<CanvasGroup>();

        playableHUD.DOFade(1f, 1f)
            .OnComplete(delegate
            {
                playableHUD.interactable = true;
                playableHUD.blocksRaycasts = true;
            });
    }

    private void HideUI()
    {
        var playableHUD = GetComponent<CanvasGroup>();

        playableHUD.DOFade(0f, 1f)
            .OnStart(delegate
            {
                playableHUD.interactable = false;
                playableHUD.blocksRaycasts = false;
            });
    }
}
