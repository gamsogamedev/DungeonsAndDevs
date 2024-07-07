using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField, Foldout("--- Grid Creation ---")] private int width, height;
    [SerializeField, Foldout("--- Grid Creation ---")] private GameObject cellPrefab;

    private Dictionary<Vector2, Cell> _coordToCell;
    private Dictionary<Cell, Vector2> _cellToCoord;
    
    private Cell _activeCell;

    public static readonly UnityEvent<Cell> OnSelect = new(), OnDeselect = new();
    public static readonly UnityEvent GridClear = new();

    private void Awake() => Instance = this;

    private void Start()
    {
        GenerateGrid();
        OnSelect.AddListener(SetActiveCell);
    }

    private void GenerateGrid()
    {
        _cellToCoord = new Dictionary<Cell, Vector2>();
        _coordToCell = new Dictionary<Vector2, Cell>();
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var cell = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, this.transform);
                cell.name = $"Cell {x} {y}";
                
                var cellRef = cell.GetComponent<Cell>();
                cellRef.InitCell();

                var coord = new Vector2(x, y);
                _cellToCoord.Add(cellRef, coord);
                _coordToCell.Add(coord, cellRef);
            }
        }

        if (Camera.main is null)
        {
            Debug.LogError("No camera in the scene");
            return;
        }
            
        Camera.main.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, -10);

        // USE LATER (MAYBE)
        // var entities = Resources.LoadAll<ScriptableEntity>("Teste");
        // foreach (var ent in entities)
        // {
        //     Instantiate(ent.entityPrefab);
        // }
    }

    private void SetActiveCell(Cell cell) 
    {
        _activeCell?.DeselectCell();
        _activeCell = cell;
    }

    public void ShowRadius(Cell center, int radius)
    {
        if (radius <= 0) return;
        
        var radiusCenter = _cellToCoord[center];
        for (var i = 0; i <= radius; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                var coordXPositive = radiusCenter.x + j;
                var coordYPositive = radiusCenter.y + (i - j);
                var coordXNegative = radiusCenter.x - j;
                var coordYNegative = radiusCenter.y - (i - j);

                var validXPositive = coordXPositive < width;
                var validYPositive = coordYPositive < height;
                var validXNegative = coordXNegative >= 0;
                var validYNegative = coordYNegative >= 0;

                if (validXPositive && validYPositive)
                    _coordToCell[new Vector2(coordXPositive, coordYPositive)].MarkCellAsWalkable();
                
                if(validXNegative && validYPositive) 
                    _coordToCell[new Vector2(coordXNegative, coordYPositive)].MarkCellAsWalkable();

                if (validXPositive && validYNegative) 
                    _coordToCell[new Vector2(coordXPositive, coordYNegative)].MarkCellAsWalkable();

                if (validXNegative && validYNegative) 
                    _coordToCell[new Vector2(coordXNegative, coordYNegative)].MarkCellAsWalkable();
            }
        }
    }

    public Cell[] GetPath(Cell startPoint, Cell finishPoint)
    {
        var path = new Cell[10];
        return path;
    }
}
