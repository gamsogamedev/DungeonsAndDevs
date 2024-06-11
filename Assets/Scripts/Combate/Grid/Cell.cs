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
    
    [SerializeField, Foldout("State Indicators")]
    private GameObject hoveredInd, selectedInd;
    
    public readonly UnityEvent CellSelected = new(), CellDeselected = new();
    
    public void InitCell()
    {
        CellSelected.AddListener(SelectCell);
        CellDeselected.AddListener(DeselectCell);
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
        hoveredInd.SetActive(false);
        switch (_currentState)
        {
            case CellState.Hover:
            case CellState.Idle:
                CellSelected?.Invoke();
                break;
            case CellState.Selected:
                CellDeselected?.Invoke();
                break;
        }
    }

    public void SelectCell()
    {
        _currentState = CellState.Selected;
        selectedInd.SetActive(true);
        
        GridManager.OnSelect?.Invoke(this);
    }
    public void DeselectCell()
    {
        _currentState = CellState.Idle;
        selectedInd.SetActive(false);

        GridManager.OnDeselect?.Invoke(this);
    }
}
