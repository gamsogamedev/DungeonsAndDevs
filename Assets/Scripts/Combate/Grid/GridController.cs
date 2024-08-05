using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController
{
    private static GridManager grid;
    public static void SetGrid(GridManager manager) => grid = manager; 
    
    public static List<Cell> GetRadius(Cell center, int radius)
    {
        List<Cell> cellsInRadius = new List<Cell>();
        if (radius <= 0) return null;
        
        var radiusCenter = center.cellCoord;
        for (var i = 0; i <= radius; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                var coordXPositive = radiusCenter.x + j;
                var coordYPositive = radiusCenter.y + (i - j);
                var coordXNegative = radiusCenter.x - j;
                var coordYNegative = radiusCenter.y - (i - j);

                var validXPositive = coordXPositive < grid.GetGridDimensions().x;
                var validYPositive = coordYPositive < grid.GetGridDimensions().y;
                var validXNegative = coordXNegative >= 0;
                var validYNegative = coordYNegative >= 0;

                if (validXPositive && validYPositive)
                    cellsInRadius.Add(grid.getCellAtCoord(coordXPositive, coordYPositive));
                
                if(validXNegative && validYPositive) 
                    cellsInRadius.Add(grid.getCellAtCoord(coordXNegative, coordYPositive));

                if (validXPositive && validYNegative) 
                    cellsInRadius.Add(grid.getCellAtCoord(coordXPositive, coordYNegative));

                if (validXNegative && validYNegative) 
                    cellsInRadius.Add(grid.getCellAtCoord(coordXNegative, coordYNegative));
            }
        }

        cellsInRadius.Remove(center);
        return cellsInRadius;
    }    
    
    #region Pathfinding
    public static List<Cell> GetPath(Cell startPoint, Cell finishPoint)
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
                if (closedList.Contains(neighbor)) continue;
                if (!neighbor._currentState.HasFlag(CellState.Walkable)) continue;
                
                
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
    
    private static int Distance(Cell a, Cell b) => Mathf.Abs(b.cellCoord.x - a.cellCoord.x) + Mathf.Abs(b.cellCoord.y - a.cellCoord.y);
    
    private static List<Cell> RetrievePath(Cell finishPoint)
    {
        List<Cell> path = new() { finishPoint };

        var currentNode = finishPoint;
            
        while(currentNode.previousCell is not null)
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
