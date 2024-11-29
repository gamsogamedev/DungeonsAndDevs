using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    private bool _canInteract;
    [SerializeField] private StartGameHUD _startGameHUD;

    public static UnityEvent OnInteractionTriggered = new();
    public static UnityEvent OnInteractionEnded = new();

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        _canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        _canInteract = false;
    }
    
    private void Update()
    {
        if (!_startGameHUD.GetComponent<Canvas>().enabled)
        {
            OnInteractionEnded?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.E) && _canInteract) {
            OnInteractionTriggered?.Invoke();
            
            _startGameHUD.Open();
        }
    }
}
