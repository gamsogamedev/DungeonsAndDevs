using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridController
{
    private static GridManager grid;
    public static void SetGrid(GridManager manager) => grid = manager;

    public static Cell GetCellAt(Vector2Int coord)
    {
        var dim = grid.GetGridDimensions();
        var finalX = coord.x; var finalY = coord.y;
        
        if (coord.x > dim.x)
            finalX = coord.x;
        else if (coord.x < 0)
            finalX = 0;
        
        if (coord.y > dim.y)
            finalY = coord.y;
        else if (coord.y < 0)
            finalY = 0;
        
        return grid.getCellAtCoord(finalX, finalY);
    }
    
    public static List<Cell> GetRing(Cell center, int radius)
    {
        List<Cell> cellsInRing = new List<Cell>();
        if (radius < 0) return cellsInRing;

        var ringCenter = center.cellCoord;
        for (var j = 0; j <= radius; j++)
        {
            var coordXPositive = ringCenter.x + j;
            var coordYPositive = ringCenter.y + (radius - j);
            var coordXNegative = ringCenter.x - j;
            var coordYNegative = ringCenter.y - (radius - j);

            var validXPositive = coordXPositive < grid.GetGridDimensions().x;
            var validYPositive = coordYPositive < grid.GetGridDimensions().y;
            var validXNegative = coordXNegative >= 0;
            var validYNegative = coordYNegative >= 0;

            if (validXPositive && validYPositive)
                cellsInRing.Add(grid.getCellAtCoord(coordXPositive, coordYPositive));
                
            if(validXNegative && validYPositive) 
                cellsInRing.Add(grid.getCellAtCoord(coordXNegative, coordYPositive));

            if (validXPositive && validYNegative) 
                cellsInRing.Add(grid.getCellAtCoord(coordXPositive, coordYNegative));

            if (validXNegative && validYNegative) 
                cellsInRing.Add(grid.getCellAtCoord(coordXNegative, coordYNegative));
        }
        
        return cellsInRing.Distinct().ToList();
    }
    
    public static List<Cell> GetRadius(Cell center, int radius)
    {
        List<Cell> cellsInRadius = new List<Cell>();
        if (radius < 0) return cellsInRadius;
        
        var radiusCenter = center.cellCoord;
        for (var i = 0; i <= radius; i++)
        {
            cellsInRadius.AddRange(GetRing(center, i));
        }
        
        return cellsInRadius.Distinct().ToList();
    }

    public static List<Cell> GetLine(Cell center, int range, Vector2Int direction)
    {
        List<Cell> cells = new List<Cell>();
        if (range <= 0) return null;
        
        var lineCenter = center.cellCoord;

        for (int i = 1; i <= range; i++) 
        {
            var cellCoord = lineCenter + (direction * i);
            if (cellCoord.x < 0 || cellCoord.x > grid.GetGridDimensions().x - 1) continue;
            if (cellCoord.y < 0 || cellCoord.y > grid.GetGridDimensions().y - 1) continue;
            
            cells.Add(grid.getCellAtCoord(cellCoord.x, cellCoord.y));
        }
        
        return cells;
    }
    
    public static List<Cell> GetCross(Cell center, int range)
    {
        List<Cell> cells = new List<Cell>();
        if (range <= 0) return null;
        
        cells.AddRange(GetLine(center, range, Vector2Int.up));
        cells.AddRange(GetLine(center, range, Vector2Int.right));
        cells.AddRange(GetLine(center, range, Vector2Int.down));
        cells.AddRange(GetLine(center, range, Vector2Int.left));
        
        return cells;
    }

    public static Cell GetNeighbor(Cell center, Vector2Int neighborDirection)
    {
        return GetLine(center, 1, neighborDirection)[0];
    }
    
    #region Pathfinding
    public static List<Cell> GetPath(Cell startPoint, Cell finishPoint, CellState stateFilter = CellState.Walkable, bool avoidEntitites = true)
    {
        var openList = new List<Cell> { startPoint };
        var closedList = new List<Cell>();
                    
        startPoint.gCost = 0;
        startPoint.hCost = Distance(startPoint, finishPoint);
            
        while(openList.Count > 0)
        {
            Cell currentCell = openList.OrderBy(node => node.fCost).First();
            
            if (currentCell == finishPoint)
            {
                var path = RetrievePath(finishPoint);
                
                var cellsRead = new List<Cell>();
                cellsRead.AddRange(openList);
                cellsRead.AddRange(closedList);
                foreach (var c in cellsRead) 
                    c.ClearPath();
                
                return path;
            }

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            foreach(Cell neighbor in GetRadius(currentCell, 1))
            {
                if (closedList.Contains(neighbor)) continue;
                if (!neighbor._currentState.HasFlag(stateFilter)) continue;
                if (avoidEntitites && neighbor._entityInCell is not null) continue;
                
                var newGcost = currentCell.gCost + 1;
                if (newGcost >= neighbor.gCost) continue;

                neighbor.previousCell = currentCell;
                neighbor.gCost = newGcost;
                neighbor.hCost = Distance(neighbor, finishPoint);

                if(!openList.Contains(neighbor))
                    openList.Add(neighbor);
            }
        }
        
        return null;
    }
    
    public static int Distance(Cell a, Cell b) => Mathf.Abs(b.cellCoord.x - a.cellCoord.x) + Mathf.Abs(b.cellCoord.y - a.cellCoord.y);
    
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

    #region Auxiliar

    public static List<BaseEntity> GetEntitiesOnGrid(EntityType type = EntityType.All)
    {
        var entities = new List<BaseEntity>();
        var dim = grid.GetGridDimensions();
        
        for (var x = 0; x < dim.x; x++)
        {
            for (var y = 0; y < dim.y; y++)
            {
                var cellAt = grid.getCellAtCoord(x, y);
                if (cellAt._entityInCell is not null) 
                    entities.Add(cellAt._entityInCell);
            }
        }

        if (type == EntityType.All)
            return entities;
        else
            return entities.Where(e => e.EntityInfo.entityType == type).ToList();
    }

    #endregion
}
