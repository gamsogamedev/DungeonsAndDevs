using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Flags] public enum CellState {
    Idle        = 0, 
    Hover       = 1 << 0,
    Selected    = 1 << 1,
    
    Walkable    = 1 << 2,
    Path        = 1 << 3,
    
    Range       = 1 << 4,
    Target      = 1 << 5
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
        cellState_Path,
        cellState_Range,
        cellState_Target;
            

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
        GridManager.GridClear.AddListener(SetCellAsIdle);
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
        cellState_Hover     .SetActive(_currentState.HasFlag(CellState.Hover));
        cellState_Selected  .SetActive(_currentState.HasFlag(CellState.Selected));
        cellState_Walkable  .SetActive(_currentState.HasFlag(CellState.Walkable));
        cellState_Path      .SetActive(_currentState.HasFlag(CellState.Path));
        cellState_Range     .SetActive(_currentState.HasFlag(CellState.Range));
        cellState_Target    .SetActive(_currentState.HasFlag(CellState.Target));
    }
    
    private void ClearCell()
    {
        cellState_Idle.SetActive(true);
        cellState_Hover.SetActive(false);
        cellState_Selected.SetActive(false);
        cellState_Path.SetActive(false);
        cellState_Walkable.SetActive(false);
        cellState_Range.SetActive(false);
        cellState_Target.SetActive(false);
    }
    
    private void OnMouseEnter()
    {
        if (_currentState.HasFlag(CellState.Selected)) return;
        if (_currentState.HasFlag(CellState.Walkable))
        {
            var path = GridController.GetPath(CombatManager.SelectedEntity.currentCell, this);
            foreach (var pathCell in path)
            {
                pathCell.SetCellAsPath();   
            }
        }
        if (_currentState.HasFlag(CellState.Range))
        {
            var areaOfEffect = CombatManager.SelectedSkill.PreviewSkill(this);
            foreach (var aoeCell in areaOfEffect)
            {
                aoeCell.SetCellAsTarget();     
            }
        }
        else SetCellAsHover();
    }

    private void OnMouseExit()
    {
        if (_currentState.HasFlag(CellState.Hover)) ResetState(CellState.Hover);
        if (_currentState.HasFlag(CellState.Path))
        {
            var path = GridController.GetPath(CombatManager.SelectedEntity.currentCell, this);
            foreach (var pathCell in path)
            {
                pathCell.ResetState(CellState.Path);   
            }
        }
        if (_currentState.HasFlag(CellState.Target))
        {
            var areaOfEffect = CombatManager.SelectedSkill.PreviewSkill(this);
            foreach (var aoeCell in areaOfEffect)
            {
                aoeCell.ResetState(CellState.Target);     
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (_entityInCell is not null)
        {
            GridManager.GridClear?.Invoke();
            _entityInCell?.EntitySelected?.Invoke();
        }
        else if (_currentState.HasFlag(CellState.Selected))
            CellDeselected?.Invoke();
        else if (_currentState.HasFlag(CellState.Walkable))
            CombatManager.SelectedEntity.MoveTowards(this);
        else if (_currentState.HasFlag(CellState.Range))
            CombatManager.SelectedSkill.OnSkillUse?.Invoke(this);
        else
        {
            GridManager.GridClear?.Invoke();
            CellSelected?.Invoke();
        }
    }

    public void SetCellAsIdle() => SetState(CellState.Idle);
    public void SetCellAsHover() => SetState(CellState.Hover, flag: true);
    public void SetCellAsWalkable() => SetState(CellState.Walkable);
    public void SetCellAsPath()
    {
        if (!_currentState.HasFlag(CellState.Walkable)) return;
        SetState(CellState.Path, flag: true);
    }
    public void SetCellAsSelected()
    {
        if (_currentState.HasFlag(CellState.Walkable))
        {
            CombatManager.SelectedEntity.MoveTowards(this);
            return;
        }
        if (_currentState.HasFlag(CellState.Walkable))
        {
            CombatManager.SelectedEntity.MoveTowards(this);
            return;
        }
        
        SetState(CellState.Selected);
        GridManager.OnSelect?.Invoke(this);
    }
    public void SetCellAsRange() => SetState(CellState.Range);
    public void SetCellAsTarget() {
        //if (!_currentState.HasFlag(CellState.Range)) return;
        SetState(CellState.Target, flag: true);
    }
    
    
    private void SetState(CellState state, bool flag = false)
    {
        if (flag) _currentState |= state;
        else _currentState = state;
        
        UpdateCell();
    }
    private void ResetState(CellState state)
    {
        if (!_currentState.HasFlag(state)) return;
        _currentState &= ~state;

        UpdateCell();
    }


    public void ClearPath()
    {
        previousCell = null;
        gCost = Int32.MaxValue;
    }
    
    private void ResetPathing()
    {
        ResetState(CellState.Walkable);
        ResetState(CellState.Path);
        previousCell = null;
        gCost = Int32.MaxValue;
    }

    
    // VISUALS -------------------------------------------------------------------------------------------
    [Space(10), SerializeField] private Animator fxAnimator;
    public void ActivateVisual(AnimatorOverrideController skillVisual, bool play, int rotation = 0)
    {
        if (!play) return;

        fxAnimator.gameObject.SetActive(true);
        fxAnimator.transform.Rotate(Vector3.forward, rotation);
        
        fxAnimator.runtimeAnimatorController = skillVisual;
        fxAnimator.Play("_FX");
        
        Invoke(nameof(ResetVisual), .5f);
    }

    public void ResetVisual()
    {
        fxAnimator.runtimeAnimatorController = null;
        fxAnimator.transform.rotation = Quaternion.identity;
        fxAnimator.gameObject.SetActive(false);
    }
}
