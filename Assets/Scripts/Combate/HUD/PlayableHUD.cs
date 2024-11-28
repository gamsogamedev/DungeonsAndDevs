using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
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
            
            button.onClick.AddListener(() => CombatManager.SetAttackStage(skill));
            
            // This is probs not good
            button.interactable = true;
        }
    }

    [SerializeField] private CanvasGroup turnHUD;
    [SerializeField] private TextMeshProUGUI turnIndicator;
    [Space(20)]
    [SerializeField] private CanvasGroup playableHUD;
    [FormerlySerializedAs("skill1")] [SerializeField] private SkillVisual skill1Visual;
    [FormerlySerializedAs("skill2")] [SerializeField] private SkillVisual skill2Visual;
    [FormerlySerializedAs("skill3")] [SerializeField] private SkillVisual skill3Visual;
    [FormerlySerializedAs("skill4")] [SerializeField] private SkillVisual skill4Visual;
    [Space(5)] 
    [SerializeField] private Button movementButton;
    [Space(10)] 
    [SerializeField] private Button endTurnButton;
    [Space(10)] 
    [SerializeField] private HealthHUD activePlayerHealth;

    private void Awake()
    {
        GridManager.GridGenerated.AddListener(HideUI);
    }

    private void Start()
    {
        turnHUD.alpha = 0;
        
        CombatManager.OnEntityTurn.AddListener(UpdateHUD);
        endTurnButton.onClick.AddListener(delegate
        {
            DisableEndTurnButton();
            CombatManager.Instance.PassTurn();
        });
    }

    private void ReactivateEndTurnButton() => endTurnButton.interactable = true;
    private void DisableEndTurnButton()
    {
        endTurnButton.interactable = false;
        Invoke(nameof(ReactivateEndTurnButton), 1.5f);
    }

    private void UpdateHUD(BaseEntity entity)
    {
        var changeTurnAnim = DOTween.Sequence();
        changeTurnAnim.Append(turnHUD.DOFade(0f, .5f));
        changeTurnAnim.AppendCallback(() => turnIndicator.text = $"{entity.EntityInfo.name}'s\n Turn");
        changeTurnAnim.Append(turnHUD.DOFade(1f, .5f));
        
        if (entity.EntityInfo.entityType != EntityType.Playable)
        {
            HideUI();
            return;
        }

        movementButton.onClick.AddListener(() => CombatManager.SetMovementStage(entity));
        
        activePlayerHealth.Init(entity);
        entity.HealthUpdated?.Invoke(entity.healthPercentage);
        
        var playableEntity = (PlayableEntity) entity;
        skill1Visual.SetupVisual(playableEntity.skill1);
        skill2Visual.SetupVisual(playableEntity.skill2);
        skill3Visual.SetupVisual(playableEntity.skill3);
        skill4Visual.SetupVisual(playableEntity.skill4);
        ShowUI();
    }

    private void ShowUI()
    {
        playableHUD.DOFade(1f, 1f)
            .OnComplete(delegate
            {
                playableHUD.interactable = true;
                playableHUD.blocksRaycasts = true;
            });
    }

    private void HideUI()
    {
        playableHUD.DOFade(0f, 1f)
            .OnStart(delegate
            {
                playableHUD.interactable = false;
                playableHUD.blocksRaycasts = false;
            });
    }
}
