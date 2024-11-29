using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonInstantiator : MonoBehaviour
{
    public ScriptableEntity_Playable entity;
    private DraggableEntity draggableEntity;

    private bool isInstantiated;
    public bool valid => isInstantiated && draggableEntity.isOnGrid;

    private void Awake() {
        isInstantiated = false;
    }

    public void HUDGeneratePlayer() {

        if (entity is null) return;

        if (!isInstantiated){
            var e = entity.GenerateEntity(); 
            draggableEntity = e.GetComponent<DraggableEntity>();
            isInstantiated = true;
            draggableEntity.SetupDrag();
        }    

        draggableEntity.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward*9;
    }

    public void HUDDragPlayer(){
        draggableEntity?.OnMouseDrag();
    }

    public void HUDMouseUpPlayer(){
        draggableEntity?.OnMouseUp();
        
        if (valid){
            var triggerEvent = this.GetComponent<EventTrigger>();
            triggerEvent.triggers = new List<EventTrigger.Entry>();
        }
    }
}