using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Flags] public enum CellState {
    Idle        = 0, 
    Hover       = 1 << 0,
    Walkable    = 1 << 1,
    Path        = 1 << 2,
    Selected    = 1 << 3
}

public class Cell : MonoBehaviour
{
    public Vector2Int cellCoord { get; private set; }
    
    public CellState _currentState { get; private set; }

    [SerializeField, Foldout("State Indicators")]
    private GameObject cellState_Idle,
        cellState_Hover, 
        cellState_Selected, 
        cellState_Walkable, 
        cellState_Path;

    [HideInInspector] public BaseEntity _entityInCell;
    
    // ---- PATHFINDING -----
    public Cell previousCell;
    public int gCost, hCost;
    public int fCost => gCost + hCost;
    
    public readonly UnityEvent CellSelected = new(), CellDeselected = new();

    public void InitCell(int xCoord, int yCoord)
    {
        cellCoord = new Vector2Int(xCoord, yCoord);
        
        CellSelected.AddListener(SetCellAsSelected);
        CellDeselected.AddListener(SetCellAsIdle);

        GridManager.OnSelect.AddListener((x) => ResetState(CellState.Walkable));
        GridManager.GridClear.AddListener(ClearCell);
        BaseEntity.OnEntityMove.AddListener(ResetPathing);

        
        ResetPathing();
        SetCellAsIdle();
    }
    
    private void UpdateCell()
    {
        if (_currentState == CellState.Idle)
        {
            ClearCell();
            return;
        }
        cellState_Hover.SetActive(_currentState.HasFlag(CellState.Hover));
        cellState_Selected.SetActive(_currentState.HasFlag(CellState.Selected));
        cellState_Walkable.SetActive(_currentState.HasFlag(CellState.Walkable));
        cellState_Path.SetActive(_currentState.HasFlag(CellState.Path));
    }
    
    private void OnMouseEnter()
    {
        if (_currentState.HasFlag(CellState.Selected)) return;
        SetCellAsHover();
    }

    private void OnMouseExit()
    {
        if (!_currentState.HasFlag(CellState.Hover)) return;
        
        ResetState(CellState.Hover);
    }

    private void OnMouseUpAsButton()
    {
        GridManager.GridClear?.Invoke();

        if (_entityInCell is not null) _entityInCell?.EntitySelected?.Invoke();
        else if (_currentState.HasFlag(CellState.Selected))
            CellDeselected?.Invoke();
        else CellSelected?.Invoke();
    }

    public void SetCellAsIdle() => SetState(CellState.Idle);
    public void SetCellAsHover() => SetState(CellState.Hover);
    public void SetCellAsWalkable() => SetState(CellState.Walkable);
    public void SetCellAsPath()
    {
        if (!_currentState.HasFlag(CellState.Walkable)) return;
        SetState(CellState.Path);
    }
    public void SetCellAsSelected()
    {
        if (_currentState.HasFlag(CellState.Walkable))
        {
            CombatManager.SelectedEntity.MoveTowards(this);
            return;
        }

        SetState(CellState.Idle);
        SetState(CellState.Selected);
        GridManager.OnSelect?.Invoke(this);
    }

    private void SetState(CellState state)
    {
        if (_currentState == CellState.Idle || state == CellState.Idle) _currentState = state;
        else _currentState |= state;
        
        UpdateCell();
    }

    private void ResetState(CellState state)
    {
        if (!_currentState.HasFlag(state)) return;

        var fullState = CellState.Hover | CellState.Selected | CellState.Path | CellState.Walkable;
        _currentState &= (fullState ^ state);

        UpdateCell();
    }
    
    private void ClearCell()
    {
        cellState_Hover.SetActive(false);
        cellState_Selected.SetActive(false);
        cellState_Path.SetActive(false);
        cellState_Walkable.SetActive(false);
    }

    private void ResetPathing()
    {
        ResetState(CellState.Walkable);
        previousCell = null;
        gCost = Int32.MaxValue;
    }
}
