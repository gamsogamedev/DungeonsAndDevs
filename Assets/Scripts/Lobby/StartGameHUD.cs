using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameHUD : MonoBehaviour
{
    private Canvas hudCanvas;
    
    [SerializeField] private List<EntitySlotHUD> availables;
    [SerializeField] private List<EntitySlotHUD> party;

    [SerializeField] private Button startButton;
    
    private void Awake()
    {
        startButton.onClick.AddListener(StartRun);
        
        EntitySlotHUD.EntitySelected.AddListener(ParseParty);
        UpdatePartyHUD();

        hudCanvas = GetComponent<Canvas>();
        Close();
    }
    
    public void Open()
    {
        hudCanvas.enabled = true;
    }
    public void Close()
    {
        hudCanvas.enabled = false;
    }

    private void ParseParty(EntitySlotHUD e)
    {
        if (e.GetEntity() is null) return;
        
        var p = GameManager.Instance.party;
        if (p.Contains(e.GetEntity()))
        {
            
            if (e.isPartySlot)
            { 
                if (e.GetEntity().entityName == "Jogador") return;
                
                availables.First(slot => slot.GetEntity() == e.GetEntity()).MarkAsSelected(false);
            }
            else
            {
                e.MarkAsSelected(false);
            }
        
            p.Remove(e.GetPlayableEntity());
        }
        else
        {
            if (e.isPartySlot) return;
            if (p.Count == GameManager.Instance.CurrentPartySize) return;
            
            e.MarkAsSelected(true);
            p.Add(e.GetPlayableEntity());
        }
        
        UpdatePartyHUD();
    }

    private void UpdatePartyHUD()
    {
        var p = GameManager.Instance.party;
        
        for (var i = 0; i < party.Count; i++)
        {
            if (i >= GameManager.Instance.CurrentPartySize)
            {
                party[i].SetSlotInfo(null);
            }
            else
            {
                if (i >= p.Count)
                {
                    party[i].SetSlotInfo(p[0], true);
                }
                else
                {
                    party[i].SetSlotInfo(p[i]);
                } 
            }
        }   
    }

    private void StartRun()
    {
        GameManager.Instance.ResetMapProgress();
        SceneManager.LoadScene(GameManager.GetUnlock("Tutorial") ? "Cenas/Mapa" : "TesteCombate");
    }
    
}
