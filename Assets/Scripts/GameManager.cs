using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public static readonly UnityEvent<WorldUpdate> UpdateWorldState = new();
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = null;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        WorldStateReader.Initialize();
        UpdateWorldState.AddListener(UpdateWorld);
    }

    private static void UpdateWorld(WorldUpdate update)
    {
        WorldStateReader.EditNode(update.nodeName, update.nodeValue);
        Debug.Log(WorldStateReader.RetrieveNodeValue(update.nodeName));
    }
   
}
