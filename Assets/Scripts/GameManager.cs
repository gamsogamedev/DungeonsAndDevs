using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class WorldUpdate
{
    public string nodeName;
    public string nodeValue;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public List<ScriptableEntity_Playable> party;
    public int MaxPartySize;
    public int CurrentPartySize { get; private set; }
    public void UnlockPartySlot()
    {
        if(CurrentPartySize == MaxPartySize) return;
        CurrentPartySize++;
    }

    // ----- MAP
    private int mapProgress;

    public int GetMapProgress(){
        return mapProgress;
    }

    public void AddMapProgress(){
        mapProgress++;
    }

    public void ResetMapProgress(){
        mapProgress = 0;
    }

    
    // ----- COMBAT INFO
    public static CombatSettingsScriptable currentCombatInfo;
    public static void SetNewGame(CombatSettingsScriptable s) => currentCombatInfo = s;
    
    // ----- CURRENCY
    public static int currency;
    public static readonly UnityEvent<int> CurrencyUpdated = new();
    
    public static readonly UnityEvent<WorldUpdate> UpdateWorldState = new();
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        PlayerPrefs.DeleteKey("Tutorial");
        
        SetUnlock("Jogador");
        CurrentPartySize = 2;

        mapProgress = 0;        
        WorldStateReader.Initialize();
        UpdateWorldState.AddListener(UpdateWorld);
    }

    public static void UpdateCurrency(int value)
    {
        currency += value;
        CurrencyUpdated?.Invoke(currency);
    }
    
    private static void UpdateWorld(WorldUpdate update)
    {
        WorldStateReader.EditNode(update.nodeName, update.nodeValue);
        Debug.Log(WorldStateReader.RetrieveNodeValue(update.nodeName));
    }

    // ----- UNLOCK
    public static void SetUnlock(string unlockName) => PlayerPrefs.SetInt(unlockName, 1);
    public static bool GetUnlock(string unlockName) => PlayerPrefs.HasKey(unlockName);

    [Button("Unlock")]
    public void UnlockAll()
    {
        ClearPrefs();
        SetUnlock("Mary");
        SetUnlock("Daniboy");
        SetUnlock("Ast");
        SetUnlock("Giw");
        SetUnlock("Patu");
    }

    [Button("Clean Unlocks")]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        SetUnlock("Jogador");
    }
}
