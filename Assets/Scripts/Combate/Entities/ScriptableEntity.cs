using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    Playable,
    Hostile
} 

[CreateAssetMenu(menuName = "Entity/New Entity", fileName = "Entity")]
public class ScriptableEntity : ScriptableObject
{
    public EntityType entityType;
    public BaseEntity entityPrefab;
    
    
}