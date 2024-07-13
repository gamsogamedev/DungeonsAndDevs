using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
    public enum CellState {Idle, Hover, Selected}
    
    private CellState _currentState;
    [HideInInspector] public BaseEntity _entityInCell;
    
    [SerializeField, Foldout("State Indicators")]
    private GameObject hoveredInd, selectedInd, walkableInd, pathInd;

    public Cell previousCell;
    public int gCost, hCost;
    public int fCost => gCost + hCost;
    

    public bool _canBeWalked;
    
    public readonly UnityEvent CellSelected = new(), CellDeselected = new();

    public void InitCell()
    {
        CellSelected.AddListener(SelectCell);
        CellDeselected.AddListener(DeselectCell);

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
        
        _currentState = CellState.Hover;
        hoveredInd.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (_currentState != CellState.Hover) return;
        
        _currentState = CellState.Idle;
        hoveredInd.SetActive(false);
    }

    private void OnMouseUpAsButton()
    {
        GridManager.GridClear?.Invoke();
        hoveredInd.SetActive(false);
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

    public void MarkCellAsWalkable()
    {
        if (_entityInCell is null) _canBeWalked = true;
        walkableInd.SetActive(true);
    }

    public void MarkCellAsPath()
    {
        if (!_canBeWalked) return; 
        pathInd.SetActive(true);
    }
    
    public void SelectCell()
    {
        if (_canBeWalked)
        {
            CombatManager.SelectedEntity.MoveTowards(this);
            return;
        }
        
        _currentState = CellState.Selected;
        selectedInd.SetActive(true);
        
        GridManager.OnSelect?.Invoke(this);
    }
    
    public void DeselectCell()
    {
        if (_entityInCell is not null) _entityInCell.isSelected = false;
        _currentState = CellState.Idle;
        selectedInd.SetActive(false);
        
        GridManager.OnDeselect?.Invoke(this);
    }

    private void ClearCell()
    {
        _currentState = CellState.Idle;
        selectedInd.SetActive(false);
        
        pathInd.SetActive(false);
        walkableInd.SetActive(false);
    }
}
