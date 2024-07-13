using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/New Entity", fileName = "Entity")]
public class ScriptableEntity : ScriptableObject
{
    public EntityType entityType;
    public BaseEntity entityPrefab;
}



public enum EntityType
{
    Playable,
    Hostile
} 