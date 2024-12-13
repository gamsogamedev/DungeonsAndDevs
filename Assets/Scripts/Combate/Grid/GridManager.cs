using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField, Foldout("--- Grid Creation ---")] private int width, height;
    [SerializeField, Foldout("--- Grid Creation ---")] private Cell cellPrefab;
    public Vector2Int GetGridDimensions() => new Vector2Int(width, height);
    
    public Dictionary<Vector2Int, Cell> _coordToCell { get; private set; }
    public Cell getCellAtCoord(int x, int y) => _coordToCell[new Vector2Int(x, y)];
    
    public Cell _activeCell { get; private set; }

    public static readonly UnityEvent GridGenerated = new();
    
    public static readonly UnityEvent<Cell> OnSelect = new(), OnDeselect = new();
    public static readonly UnityEvent GridClear = new();

    private void Awake()
    {
        Instance = this;
        OnSelect.AddListener(SetActiveCell);
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _coordToCell = new Dictionary<Vector2Int, Cell>();
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var cell = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, this.transform);
                
                cell.name = $"Cell {x} {y}";
                cell.InitCell(x, y);
                _coordToCell.Add(cell.cellCoord, cell);
                
                var cellBounds = cell.GetComponent<SpriteRenderer>().sprite.bounds.size;
                var cellPos = new Vector3(x * (cellBounds.x - (cellBounds.x - cellBounds.y) * .8f) * cell.transform.localScale.x, 
                                    y * cellBounds.y * cell.transform.localScale.y);
                cell.transform.position = cellPos;
                cell.transform.position += (Vector3.right * y * (cellBounds.x / 4.7f) * cell.transform.localScale.x);
            }
        }

        if (Camera.main is null)
        {
            Debug.LogError("No camera in the scene");
            return;
        }

        Camera.main.orthographicSize = 7.5f;
        Camera.main.transform.position = new Vector3((width / 2f) + 1.7f, (height / 2f) - 1f, -10);
        
        GridController.SetGrid(this);
        GridGenerated?.Invoke();
    }

    private void SetActiveCell(Cell cell) 
    {
        _activeCell?.SetCellAsIdle();
        _activeCell = cell;
        
        _activeCell.CellDeselected.AddListener(() => _activeCell = null);
    }
    
    public static void ShowRadiusAsWalkable(BaseEntity ent) =>
        ShowRadiusAsWalkable(ent.currentCell, ent.currentMovement);
    public static void ShowRadiusAsWalkable(Cell center, int radius)
    {
        GridClear?.Invoke();
        var cellInRadius = GridController.GetRadius(center, radius);
        if (cellInRadius is null || !cellInRadius.Any()) return;
        
        foreach (var cell in cellInRadius)
        {
            cell.SetCellAsWalkable();
        }
    }

    public static void ShowRadiusAsRange(BaseEntity ent, Skill sk) =>
        ShowRadiusAsRange(ent.currentCell, sk.SkillRange);
    public static void ShowRadiusAsRange(Cell center, Range range)
    {
        GridClear?.Invoke();
        
        var cellInRadius = range.GetRange(center);
        if (cellInRadius is null || !cellInRadius.Any()) return;
        
        foreach (var cell in cellInRadius)
        {
            cell.SetCellAsRange();
        }
    }
    
    public static void ShowRadiusAsPreview(Cell center, int radius)
    {
        var cellInRadius = GridController.GetRadius(center, radius);
        if (cellInRadius is null || !cellInRadius.Any()) return;
        
        foreach (var cell in cellInRadius)
        {
            cell.SetCellAsTarget();
        }
    }

    public static void ClearGrid() => GridClear?.Invoke();
}
