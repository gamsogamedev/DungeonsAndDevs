using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameHUD : MonoBehaviour
{
    [SerializeField] private List<StartGameSlotHUD> availables;
    [SerializeField] private List<StartGameSlotHUD> party;

    [SerializeField] private Button startButton;
    
    private void Awake()
    {
        startButton.onClick.AddListener(StartRun);
        
        StartGameSlotHUD.EntitySelected.AddListener(ParseParty);
        UpdatePartyHUD();

        Close();
    }
    
    public void Open()
    {
        GetComponent<Canvas>().enabled = true;
    }
    public void Close()
    {
        GetComponent<Canvas>().enabled = false;
    }

    private void ParseParty(StartGameSlotHUD e)
    {
        var p = GameManager.Instance.party;
        if (p.Contains(e.GetEntity()))
        {
            e.MarkAsSelected(false);
            p.Remove(e.GetEntity());
        }
        else
        {
            if (p.Count == GameManager.Instance.CurrentPartySize) return;
            
            e.MarkAsSelected(true);
            p.Add(e.GetEntity());
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
