using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class sceneswapmanager : MonoBehaviour
 //ideia para fazer a localização do player na cena e colocar ele no spawn correto , tentativa
{
    /*
    public List<GameObject> sceneList;public static sceneswapmanager instance;
    
    private GameObject _Player;
    private Collider2D _PlayerColl;
    private Collider2D _doorColl;
    private Vector3 _playerspawnpositions;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } 
        _Player = GameObject.FindGameObjectWithTag("Player");
        _PlayerColl = _Player.GetComponent<Collider2D>();
    }
     private void OnEnable()
     {
         SceneManager.sceneLoaded += OnSceneLoded;
     }
     private void OnDisable()
     {
         SceneManager.sceneLoaded -= OnSceneLoded;
     }
    
   private void OnSceneLoded(Scene scene, LoadSceneMode mode){
    if(_LoadFromDoor){
        FindDoor(_doorToSpawnTo);
        _Player.transform.position = _playerspawnposition;
        _LoadfromDoor = false;
    }

   }
   private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt DoorSpawnNumber)
   {
    DoorTriggerInteraction[] doors = FindObjectOfType<DoorTriggerInteraction>();

    for(int i = 0; i < doors.Length; i++){
        if(doors[i].CurrentDoorPosition == DoorSpawnNumber){
            _doorColl = doors[i].GetComponent<Collider2D>();
            CalculateSpawnPosition();
            return;
        }
    }
   }

   private void CalculateSpawnPosition()
   {
    float colliderHeight = _PlayerColl.bounds.extents.y;
    _playerSpawnPosition = _doorColl.transform.position - new Vector3(0f, colliderHeight, 0f);
   }
   */
}
