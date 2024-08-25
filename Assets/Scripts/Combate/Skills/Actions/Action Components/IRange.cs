using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public enum RangeType
{
    None,
    Self,
    Line,
    Cross,
    Radius
}

[System.Serializable]
public class Range
{
    public void EditRange()
    {
        switch (rangeType)
        {
            default:
            case RangeType.Self:
                range = new Range_Single();
                break;
            case RangeType.Line:
                range = new Range_Line();
                break;
            case RangeType.Cross:
                range = new Range_Cross();
                break;
            case RangeType.Radius:
                range = new Range_Radius();
                break;
        }
    }
    [AllowNesting, OnValueChanged(nameof(EditRange))] public RangeType rangeType = RangeType.None;
    [SerializeReference] public IRange range;

    public List<Cell> GetRange(Cell center) => range.GetRange(center);
}

[System.Serializable]
public class IRange
{
    public int range;
    
    public virtual List<Cell> GetRange(Cell center) =>  null;
}

[System.Serializable]
public class Range_Single : IRange
{
    public override List<Cell> GetRange(Cell center)
    {
        return new List<Cell>() {center};
    }
}

[System.Serializable]
public class Range_Line : IRange
{
    [Dropdown(nameof(GetVectorValues)), AllowNesting] public Vector2Int direction;
    private DropdownList<Vector2Int> GetVectorValues()
    {
        return new DropdownList<Vector2Int>()
        {
            { "Right",   Vector2Int.right },
            { "Left",    Vector2Int.left  },
            { "Up",      Vector2Int.up    },
            { "Down",    Vector2Int.down  },
        };
    }
    
    public override List<Cell> GetRange(Cell center)
    {
        return GridController.GetLine(center, range, direction);
    }
}

[System.Serializable]
public class Range_Cross : IRange
{
    public override List<Cell> GetRange(Cell center)
    {
        return GridController.GetCross(center, range);
    }
}

[System.Serializable]
public class Range_Radius : IRange
{
    public override List<Cell> GetRange(Cell center)
    {
        return GridController.GetRadius(center, range);
    }
}