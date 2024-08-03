using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    public Vector2Int cellCoord { get; private set; }
 
    public enum CellState {Idle, Hover, Selected}
    private CellState _currentState;

    [SerializeField, Foldout("State Indicators")]
    private GameObject cellState_Idle,
        cellState_Hover, 
        cellState_Selected, 
        cellState_Walkable, 
        cellState_Path;
    private GameObject currentState;

    [HideInInspector] public BaseEntity _entityInCell;
    
    // ---- PATHFINDING -----
    public Cell previousCell;
    public int gCost, hCost;
    public int fCost => gCost + hCost;
    

    public bool _canBeWalked;
    public readonly UnityEvent CellSelected = new(), CellDeselected = new();

    public void InitCell(int xCoord, int yCoord)
    {
        cellCoord = new Vector2Int(xCoord, yCoord);
        
        CellSelected.AddListener(SetCellAsSelected);
        CellDeselected.AddListener(SetCellAsIdle);

        GridManager.OnSelect.AddListener((x) => _canBeWalked = false);
        GridManager.GridClear.AddListener(ClearCell);
        BaseEntity.OnEntityMove.AddListener(delegate
        {
            _canBeWalked = false;
            previousCell = null;
            gCost = Int32.MaxValue;
        });

        _canBeWalked = false;
        gCost = Int32.MaxValue;
        
        _currentState = CellState.Idle;
    }
    
    private void OnMouseEnter()
    {
        if (_currentState == CellState.Selected) return;
        SetCellAsHover();
    }

    private void OnMouseExit()
    {
        if (_currentState != CellState.Hover) return;
        ClearCell();
    }

    private void OnMouseUpAsButton()
    {
        GridManager.GridClear?.Invoke();
        cellState_Hover.SetActive(false);
        switch (_currentState)
        {
            case CellState.Hover:
            case CellState.Idle:
                if (_entityInCell is null || _canBeWalked) CellSelected?.Invoke();
                else _entityInCell?.EntitySelected?.Invoke();
                break;
            case CellState.Selected:
                CellDeselected?.Invoke();
                break;
        }
    }

    public void SetCellAsWalkable()
    {
        if (_entityInCell is null) _canBeWalked = true;
        cellState_Walkable.SetActive(true);
    }
    public void SetCellAsPath()
    {
        if (!_canBeWalked) return; 
        cellState_Path.SetActive(true);
    }
    public void SetCellAsSelected()
    {
        if (_canBeWalked)
        {
            CombatManager.SelectedEntity.MoveTowards(this);
            return;
        }
        
        _currentState = CellState.Selected;
        cellState_Selected.SetActive(true);
        
        GridManager.OnSelect?.Invoke(this);
    }
    public void SetCellAsIdle()
    {
        _currentState = CellState.Idle;
        cellState_Idle.SetActive(true); 
    }
    public void SetCellAsHover()
    {
        _currentState = CellState.Hover;
        cellState_Hover.SetActive(true);
    }

    private void ClearCell()
    {
        cellState_Hover.SetActive(false);
        cellState_Selected.SetActive(false);
        cellState_Path.SetActive(false);
        cellState_Walkable.SetActive(false);
        
        SetCellAsIdle();
    }
}
