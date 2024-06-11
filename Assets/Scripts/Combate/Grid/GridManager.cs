using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    [SerializeField, Foldout("--- Grid Creation ---")] private int width, height;
    [SerializeField, Foldout("--- Grid Creation ---")] private GameObject cellPrefab;
    
    private Dictionary<Vector2, Cell> _gridInfo;
    private Cell _activeCell;

    public static readonly UnityEvent<Cell> OnSelect = new(), OnDeselect = new();
    
    private void Start()
    {
        GenerateGrid();
        OnSelect.AddListener(SetActiveCell);
    }

    private void GenerateGrid()
    {
        _gridInfo = new Dictionary<Vector2, Cell>();
        
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var cell = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity, this.transform);
                cell.name = $"Cell {x} {y}";
                
                var cellRef = cell.GetComponent<Cell>();
                cellRef.InitCell();
                _gridInfo.Add(new Vector2(x, y), cellRef);
            }
        }

        if (Camera.main is null)
        {
            Debug.LogError("No camera in the scene");
            return;
        }
            
        Camera.main.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, -10);
    }

    private void SetActiveCell(Cell cell) 
    {
        _activeCell?.DeselectCell();
        _activeCell = cell;
    }
}
