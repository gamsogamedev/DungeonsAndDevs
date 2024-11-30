using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LobbyLoad : MonoBehaviour
{
    [Serializable]
    public class ConditionalProps
    {
        public GameObject prop;
        public string conditionKey;
        public bool activateIfCondition = true;
        public bool useConditionValue;
        [AllowNesting, ShowIf(nameof(useConditionValue))] public int conditionValue;
    }

    [SerializeField] private List<ConditionalProps> _propsList;

    private void Awake()
    {
        foreach (var cp in _propsList)
        {
            cp.prop.SetActive(!cp.activateIfCondition);
        }
        
        LoadProps();
    }

    private void LoadProps()
    {
        foreach (var cp in _propsList)
        {
            if (!GameManager.GetUnlock(cp.conditionKey)) continue;
            if (cp.useConditionValue)
            {
                if (PlayerPrefs.GetInt(cp.conditionKey) == cp.conditionValue)
                {
                    cp.prop.SetActive(cp.activateIfCondition);
                }
            }
            else
            {
                cp.prop.SetActive(cp.activateIfCondition);
            }
        }
    }
}
