using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public enum TargetType
{
    None,
    Cell,
    Entity
}

[System.Serializable]
public class Target
{
    public void EditTarget()
    {
        switch (targetType)
        {
            default:
            case TargetType.Cell:
                target = new CellTarget();
                break;
            case TargetType.Entity:
                target = new EntityTarget();
                break;
        }
    }
    [AllowNesting, OnValueChanged(nameof(EditTarget))] public TargetType targetType = TargetType.None;
    [SerializeReference] public ITarget target;

    public Cell GetCell() => target.GetCell();
    public BaseEntity GetEntity() => target.GetEntity();
}

[System.Serializable]
public class ITarget
{
    public virtual Cell GetCell() => null;
    public virtual BaseEntity GetEntity() => null;
}

public class CellTarget : ITarget
{
    public Cell target;

    public override Cell GetCell() => target;
    public override BaseEntity GetEntity() => target._entityInCell;
}

public class EntityTarget : ITarget
{
    public BaseEntity target;
    
    public override Cell GetCell() => target.currentCell;
    public override BaseEntity GetEntity() => target;
}
