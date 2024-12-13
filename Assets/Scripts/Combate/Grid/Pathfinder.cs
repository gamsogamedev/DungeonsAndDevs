using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCell
{
    public Cell cellRef;
    public Vector2Int cellCoord => cellRef.cellCoord;
    
    public PathCell previousCell;
    
    public int gCost, hCost;
    public int fCost => gCost + hCost;

    public PathCell(Cell cell)
    {
        this.cellRef = cell;
        
        this.previousCell = null;
        this.gCost = Int32.MaxValue;
    }

    public void ResetPath()
    {
        this.previousCell = null;
        this.gCost = Int32.MaxValue;
    }
}

public class Pathfinder
{
    private static Dictionary<Cell, PathCell> pathGrid;
    
    public static void Init(GridManager manager)
    {
        pathGrid = new Dictionary<Cell, PathCell>();

        foreach (var cell in manager._coordToCell)
        {
            pathGrid.Add(cell.Value, new PathCell(cell.Value));
        }
    }
    
    public static List<Cell> GetPath(Cell start, Cell finish, CellState stateFilter = CellState.Walkable, bool avoidEntitites = true)
    {
        var startPoint = pathGrid[start];
        var finishPoint = pathGrid[finish];
        
        var openList = new List<PathCell> { startPoint };
        var closedList = new List<PathCell>();
                    
        startPoint.gCost = 0;
        startPoint.hCost = Distance(startPoint, finishPoint);
            
        while(openList.Count > 0)
        {
            var currentCell = openList.OrderBy(node => node.fCost).First();
            
            if (currentCell == finishPoint)
            {
                var path = RetrievePath(finishPoint);
                ResetPath(openList, closedList);
                return path;
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            foreach(PathCell neighbor in GetNeighbors(currentCell))
            {
                if (closedList.Contains(neighbor)) continue;
                if (!neighbor.cellRef._currentState.HasFlag(stateFilter)) continue;
                if (avoidEntitites && neighbor.cellRef._entityInCell is not null) continue;
                
                var newGcost = currentCell.gCost + 1;
                if (newGcost >= neighbor.gCost) continue;

                neighbor.previousCell = currentCell;
                neighbor.gCost = newGcost;
                neighbor.hCost = Distance(neighbor, finishPoint);

                if(!openList.Contains(neighbor))
                    openList.Add(neighbor);
            }
        }
        
        ResetPath(openList, closedList);
        return null;
    }
    
    private static int Distance(PathCell a, PathCell b) => Mathf.Abs(b.cellCoord.x - a.cellCoord.x) + Mathf.Abs(b.cellCoord.y - a.cellCoord.y);

    private static List<PathCell> GetNeighbors(PathCell cur)
    {
        var grid = GridManager.Instance;
        var dim = grid.GetGridDimensions();
        
        var neighbors = new List<PathCell>();
        var curX = cur.cellCoord.x; var curY = cur.cellCoord.y;
        
        if (curX + 1 < dim.x)
        {
            neighbors.Add(pathGrid[grid.getCellAtCoord(curX + 1, curY)]);   
        }
        if (curX - 1 >= 0)
        {
            neighbors.Add(pathGrid[grid.getCellAtCoord(curX - 1, curY)]);   
        }
        if (curY + 1 < dim.y)
        {
            neighbors.Add(pathGrid[grid.getCellAtCoord(curX, curY + 1)]);   
        }
        if (curY - 1 >= 0)
        {
            neighbors.Add(pathGrid[grid.getCellAtCoord(curX, curY - 1)]);   
        }

        return neighbors;
    }

    private static void ResetPath(List<PathCell> openList, List<PathCell> closedList)
    {
        foreach (var pCell in openList)
        {
            pCell.ResetPath();
        }

        foreach (var pCell in closedList)
        {
            pCell.ResetPath();
        }
    }
    
    private static List<Cell> RetrievePath(PathCell finishPoint)
    {
        List<Cell> path = new() {finishPoint.cellRef};

        var currentNode = finishPoint;
            
        while(currentNode.previousCell is not null)
        {
            var prevCell = currentNode.previousCell;
            
            path.Add(prevCell.cellRef);
            currentNode = prevCell;
        }

        path.Reverse();
        path.RemoveAt(0);
        return path;
    }
}
