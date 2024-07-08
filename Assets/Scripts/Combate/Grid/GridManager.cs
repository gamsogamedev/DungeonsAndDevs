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
    [SerializeField, Foldout("--- Grid Creation ---")] private GameObject cellPrefab;

    private Dictionary<Vector2Int, Cell> _coordToCell;
    private Dictionary<Cell, Vector2Int> _cellToCoord;
    
    public Cell _activeCell;

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
        _cellToCoord = new Dictionary<Cell, Vector2Int>();
        _coordToCell = new Dictionary<Vector2Int, Cell>();
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var cell = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, this.transform);
                cell.name = $"Cell {x} {y}";
                
                var cellRef = cell.GetComponent<Cell>();
                cellRef.InitCell();

                var coord = new Vector2Int(x, y);
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

    private List<Cell> GetRadius(Cell center, int radius)
    {
        List<Cell> cellsInRadius = new List<Cell>();
        if (radius <= 0) return null;
        
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
                    cellsInRadius.Add(_coordToCell[new (coordXPositive, coordYPositive)]);
                
                if(validXNegative && validYPositive) 
                    cellsInRadius.Add(_coordToCell[new (coordXNegative, coordYPositive)]);

                if (validXPositive && validYNegative) 
                    cellsInRadius.Add(_coordToCell[new (coordXPositive, coordYNegative)]);

                if (validXNegative && validYNegative) 
                    cellsInRadius.Add(_coordToCell[new (coordXNegative, coordYNegative)]);
            }
        }

        cellsInRadius.Remove(center);
        return cellsInRadius;
    }
    
    public void ShowRadius(Cell center, int radius)
    {
        var cellInRadius = GetRadius(center, radius);
        if (cellInRadius is null || !cellInRadius.Any()) return;
        
        foreach (var cell in GetRadius(center, radius))
        {
            cell.MarkCellAsWalkable();
        }
    }

    #region Pathfinding
    public List<Cell> GetPath(Cell startPoint, Cell finishPoint)
    {
        var openList = new List<Cell> { startPoint };
        var closedList = new List<Cell>();
                    
        startPoint.gCost = 0;
        startPoint.hCost = Distance(startPoint, finishPoint);
            
        while(openList.Count > 0)
        {
            Cell currentCell = openList.OrderBy(node => node.fCost).First();

            if (currentCell == finishPoint)
                return RetrievePath(finishPoint);

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            foreach(Cell neighbor in GetRadius(currentCell, 1))
            {
                if(closedList.Contains(neighbor)) continue;
                if (!neighbor._canBeWalked) continue;
                
                
                var newGcost = currentCell.gCost + 1;
                if (newGcost >= neighbor.gCost) continue;

                neighbor.previousCell = currentCell;
                neighbor.gCost = newGcost;
                neighbor.hCost = Distance(neighbor, finishPoint);

                if(!openList.Contains(neighbor))
                    openList.Add(neighbor);
            }
        }

        // No path found
        return null;
    }
    
    private int Distance(Cell a, Cell b) => Mathf.Abs(_cellToCoord[b].x - _cellToCoord[a].x) + Mathf.Abs(_cellToCoord[b].y - _cellToCoord[a].y);
    
    private List<Cell> RetrievePath(Cell finishPoint)
    {
        List<Cell> path = new() { finishPoint };

        var currentNode = finishPoint;
            
        while(currentNode.previousCell != null)
        {
            path.Add(currentNode.previousCell);
            currentNode = currentNode.previousCell;
        }

        path.Reverse();
        path.RemoveAt(0);
        return path;
    }

    #endregion
}
