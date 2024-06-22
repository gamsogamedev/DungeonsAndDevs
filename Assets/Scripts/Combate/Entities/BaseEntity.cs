using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour
{
    public string entityName;
    public EntityType entityType;
    public Cell currentCell;
    
    public int movementRange;
}
