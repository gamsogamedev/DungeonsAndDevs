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

        public void SetupVisual(Skill skill)
        {
            icon.sprite = skill.skillInfo.skillIcon;
            cooldownOverlay.fillAmount = 0;
            
            button.onClick.AddListener(() => skill.OnSkillSelected?.Invoke());
        }
    }
    
    [FormerlySerializedAs("skill1")] [SerializeField] private SkillVisual skill1Visual;
    [FormerlySerializedAs("skill2")] [SerializeField] private SkillVisual skill2Visual;
    [FormerlySerializedAs("skill3")] [SerializeField] private SkillVisual skill3Visual;
    [FormerlySerializedAs("skill4")] [SerializeField] private SkillVisual skill4Visual;

    private void Start()
    {
        CombatManager.OnEntitySelected.AddListener(UpdateHUD);
    }

    private void UpdateHUD(BaseEntity entity)
    {
        // TEMPORARY !!!
        if (CombatManager.Instance.currentStage != CombatState.PlayerAttack)
        {
            HideUI();
            return;
        }
        
        if (entity.EntityInfo.entityType != EntityType.Playable)
        {
            HideUI();
            return;
        }
        
        var playableEntity = (PlayableEntity) entity;
        skill1Visual.SetupVisual(playableEntity.skill1);
        skill2Visual.SetupVisual(playableEntity.skill2);
        skill3Visual.SetupVisual(playableEntity.skill3);
        skill4Visual.SetupVisual(playableEntity.skill4);
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
