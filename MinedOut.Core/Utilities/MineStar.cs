using System;
using System.Collections.Generic;
using System.Linq;
using MinedOut.Core.Logic.World.Cells;

namespace MinedOut.Core.Utilities;

public class MineStar
{
    private class CellInfo
    {
        public bool IsSafe { get; set; }
        public bool IsMine { get; set; }
        public bool IsWall { get; set; }
        public int? AdjacentMineCount { get; set; }
        public bool IsVisited { get; set; }
    }

    private readonly CellInfo[,] _grid;
    private readonly int _width;
    private readonly int _height;
    private readonly Func<Vector2I, int> _getAdjacentMineCount;

    public MineStar(int width, int height, Func<Vector2I, int> getAdjacentMineCount)
    {
        _width = width;
        _height = height;
        _getAdjacentMineCount = getAdjacentMineCount;
        _grid = new CellInfo[width, height];
        
        // Initialize grid with default cell info
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _grid[x, y] = new CellInfo();
            }
        }
    }

    public List<Vector2I> FindPath(Vector2I start, Vector2I end, Cell[,] worldGrid)
    {
        // Mark the start position as safe and visited
        MarkCellAsVisited(start, worldGrid);
        
        var openList = new List<Vector2I> { start };
        var cameFrom = new Dictionary<Vector2I, Vector2I>();
        var gScore = new Dictionary<Vector2I, int> { [start] = 0 };
        var fScore = new Dictionary<Vector2I, int> { [start] = CalculateManhattan(start, end) };

        while (openList.Count > 0)
        {
            // Get the node with lowest fScore
            var current = openList.OrderBy(pos => fScore.GetValueOrDefault(pos, int.MaxValue)).First();

            if (current.Equals(end))
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);

            foreach (var neighbor in GetSafeNeighbors(current, worldGrid))
            {
                var tentativeGScore = gScore[current] + 1;
                
                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + CalculateManhattan(neighbor, end);
                    
                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
            
            // If we've run out of safe moves, try to find the next safest cell
            if (openList.Count == 0)
            {
                var nextCell = FindNextSafestCell(start, worldGrid);
                if (nextCell != null)
                {
                    // Add a path to the next safest cell
                    var pathToNextCell = FindPathToSafestCell(current, nextCell, worldGrid);
                    if (pathToNextCell.Count > 0)
                    {
                        return pathToNextCell;
                    }
                }
            }
        }

        return new List<Vector2I>();
    }

    private List<Vector2I> FindPathToSafestCell(Vector2I start, Vector2I target, Cell[,] worldGrid)
    {
        var openList = new Queue<Vector2I>();
        var cameFrom = new Dictionary<Vector2I, Vector2I>();
        var visited = new HashSet<Vector2I> { start };
        
        openList.Enqueue(start);
        
        while (openList.Count > 0)
        {
            var current = openList.Dequeue();
            
            if (current.Equals(target))
                return ReconstructPath(cameFrom, current);
                
            foreach (var neighbor in GetNeighbors(current, worldGrid, includePotentialSafe: true))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    openList.Enqueue(neighbor);
                }
            }
        }
        
        return new List<Vector2I>();
    }
    
    private Vector2I? FindNextSafestCell(Vector2I current, Cell[,] worldGrid)
    {
        var visited = new HashSet<Vector2I>();
        var queue = new Queue<Vector2I>();
        queue.Enqueue(current);
        
        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            if (visited.Contains(pos)) continue;
            visited.Add(pos);
            
            // Check if this cell has unvisited safe neighbors
            foreach (var neighbor in GetNeighbors(pos, worldGrid, includePotentialSafe: true))
            {
                var cellInfo = _grid[neighbor.X, neighbor.Y];
                if (!cellInfo.IsVisited && !cellInfo.IsMine && !cellInfo.IsWall)
                {
                    return neighbor;
                }
                
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return null;
    }
    
    private IEnumerable<Vector2I> GetSafeNeighbors(Vector2I pos, Cell[,] worldGrid)
    {
        return GetNeighbors(pos, worldGrid)
            .Where(neighbor => 
            {
                var cellInfo = _grid[neighbor.X, neighbor.Y];
                return cellInfo.IsSafe && !cellInfo.IsVisited && !cellInfo.IsMine && !cellInfo.IsWall;
            });
    }
    
    private IEnumerable<Vector2I> GetNeighbors(Vector2I pos, Cell[,] worldGrid, bool includePotentialSafe = false)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (var i = 0; i < 4; i++)
        {
            var x = pos.X + dx[i];
            var y = pos.Y + dy[i];

            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                var cellInfo = _grid[x, y];
                
                // If we're including potentially safe cells, check if it's not a wall or known mine
                if (includePotentialSafe && !cellInfo.IsWall && !cellInfo.IsMine)
                {
                    yield return new Vector2I(x, y);
                }
                // Otherwise, only return confirmed safe cells
                else if (cellInfo.IsSafe && !cellInfo.IsWall && !cellInfo.IsMine)
                {
                    yield return new Vector2I(x, y);
                }
            }
        }
    }
    
    private void MarkCellAsVisited(Vector2I pos, Cell[,] worldGrid)
    {
        var cellInfo = _grid[pos.X, pos.Y];
        if (cellInfo.IsVisited) return;
        
        cellInfo.IsVisited = true;
        cellInfo.IsSafe = true;
        
        // Mark walls based on world grid
        cellInfo.IsWall = !worldGrid[pos.X, pos.Y].IsPassable;
        
        // Get the actual mine count from the game
        cellInfo.AdjacentMineCount = _getAdjacentMineCount(pos);
        
        // If this cell has mines around, we need to update our knowledge
        if (cellInfo.AdjacentMineCount > 0)
        {
            UpdateAdjacentCells(pos, worldGrid);
        }
        else
        {
            // If no mines around, all adjacent cells are safe
            foreach (var neighbor in GetNeighbors(pos, worldGrid, includePotentialSafe: true))
            {
                _grid[neighbor.X, neighbor.Y].IsSafe = true;
            }
        }
    }
    
    private void UpdateAdjacentCells(Vector2I pos, Cell[,] worldGrid)
    {
        var unknownNeighbors = GetNeighbors(pos, worldGrid, includePotentialSafe: true)
            .Where(n => !_grid[n.X, n.Y].IsVisited)
            .ToList();
            
        var mineCount = _grid[pos.X, pos.Y].AdjacentMineCount ?? 0;
        var confirmedMines = GetNeighbors(pos, worldGrid)
            .Count(n => _grid[n.X, n.Y].IsMine);
            
        // If all mines are accounted for, mark remaining as safe
        if (mineCount == confirmedMines)
        {
            foreach (var neighbor in unknownNeighbors)
            {
                _grid[neighbor.X, neighbor.Y].IsSafe = true;
            }
        }
        // If number of unknown cells equals remaining mines, they must all be mines
        else if (unknownNeighbors.Count == (mineCount - confirmedMines))
        {
            foreach (var neighbor in unknownNeighbors)
            {
                _grid[neighbor.X, neighbor.Y].IsMine = true;
            }
        }
    }
    
    private static List<Vector2I> ReconstructPath(Dictionary<Vector2I, Vector2I> cameFrom, Vector2I current)
    {
        var path = new List<Vector2I> { current };
        
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }
        
        path.Reverse();
        return path;
    }
    
    private static int CalculateManhattan(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}
