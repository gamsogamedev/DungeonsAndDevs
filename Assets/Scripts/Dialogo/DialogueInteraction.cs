using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    [SerializeField] private List<ScriptableDialogue> dialogues;
    private bool _canInteract;
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
        if (Input.GetKeyDown(KeyCode.E) && _canInteract) {
            DialogueManager.OnStartDialogue?.Invoke(0);
        }
    }
}
