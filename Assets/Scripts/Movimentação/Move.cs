using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{ 
    public SpriteRenderer spriteRenderer; 
    public Animator anim; 
    public float speed;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        DialogueManager.OnStartDialogue.AddListener((canmove) => this.enabled = false);

        DialogueManager.OnFinishDialogue.AddListener(() => this.enabled = true);
    }

    void Update()
    {
        var movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
        spriteRenderer.flipX = movement.x < 0f;
        
        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.magnitude);
        
        transform.position = transform.position + movement * (speed * Time.deltaTime);
    }
    
}
