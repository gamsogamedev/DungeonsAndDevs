using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persistlocation : MonoBehaviour
{
    public static persistlocation instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
