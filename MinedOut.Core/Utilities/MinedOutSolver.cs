using MinedOut.Core.Logic.World;
using MinedOut.Core.Logic.World.Cells;
using Path = MinedOut.Core.Logic.World.Cells.Path;

namespace MinedOut.Core.Utilities;

public class GameWorldOracle
{
    private static readonly Vector2I[] Directions =
    {
        new(0, 1), new(0, -1), new(1), new(-1)
    };

    private readonly GameWorld _world;

    public GameWorldOracle(GameWorld world)
    {
        _world = world;
    }

    public int GetMineCount(Vector2I pos)
    {
        return Directions.Count(dir =>
            IsValidPosition(pos + dir) &&
            _world[pos.X + dir.X, pos.Y + dir.Y] is Mine);
    }

    public bool IsValidPosition(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < _world.Width &&
               pos.Y >= 0 && pos.Y < _world.Height;
    }

    public bool IsPassable(Vector2I pos)
    {
        return IsValidPosition(pos) &&
               _world[pos.X, pos.Y] is Air or Exit or Path;
    }
}

public class MinedOutSolver
{
    public enum CellState
    {
        Unknown,
        Safe,
        Visited,
        Mine
    }

    private static readonly Vector2I[] Directions =
    {
        new(0, 1), new(0, -1), new(1), new(-1)
    };

    private readonly HashSet<Vector2I> _frontier;
    private readonly CellState[,] _grid;
    private readonly int[,] _mineCount;

    private readonly GameWorldOracle _oracle;
    private readonly List<Vector2I> _path;
    private readonly int _width, _height;

    public MinedOutSolver(GameWorld world)
    {
        _oracle = new GameWorldOracle(world);
        _width = world.Width;
        _height = world.Height;
        _grid = new CellState[_height, _width];
        _mineCount = new int[_height, _width];
        _frontier = new HashSet<Vector2I>();
        _path = new List<Vector2I>();

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
        {
            var pos = new Vector2I(x, y);
            _mineCount[y, x] = -1;

            if (!_oracle.IsPassable(pos))
                _grid[y, x] = CellState.Mine;
            else if (IsNearBottom(y)) _grid[y, x] = CellState.Safe;
        }
    }

    private bool IsNearBottom(int y)
    {
        return y >= _height - 2;
    }

    private bool IsValidPosition(Vector2I pos)
    {
        return pos.X >= 0 && pos.X < _width &&
               pos.Y >= 0 && pos.Y < _height;
    }

    private List<Vector2I> GetPassableNeighbors(Vector2I pos)
    {
        var neighbors = new List<Vector2I>(4);

        foreach (var dir in Directions)
        {
            var neighbor = pos + dir;
            if (IsValidPosition(neighbor) && _oracle.IsPassable(neighbor)) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private void AddToFrontier(Vector2I pos)
    {
        _frontier.Add(pos);
    }

    private Vector2I? GetClosestFromFrontier(Vector2I target)
    {
        if (_frontier.Count == 0) return null;

        var closest = _frontier.MinBy(pos =>
            Math.Abs(pos.X - target.X) + Math.Abs(pos.Y - target.Y));

        _frontier.Remove(closest);
        return closest;
    }

    private bool DeduceFromCell(Vector2I pos)
    {
        if (_grid[pos.Y, pos.X] != CellState.Visited)
            return false;

        var neighbors = GetPassableNeighbors(pos);
        var knownMines = neighbors.Count(n => _grid[n.Y, n.X] == CellState.Mine);
        var unknownNeighbors = neighbors.Where(n => _grid[n.Y, n.X] == CellState.Unknown).ToList();
        var totalMines = _mineCount[pos.Y, pos.X];
        var remainingMines = totalMines - knownMines;

        var madeDeduction = false;

        if (remainingMines == 0)
            foreach (var neighbor in unknownNeighbors)
            {
                _grid[neighbor.Y, neighbor.X] = CellState.Safe;
                AddToFrontier(neighbor);
                madeDeduction = true;
            }
        else if (unknownNeighbors.Count == remainingMines && remainingMines > 0)
            foreach (var neighbor in unknownNeighbors)
            {
                _grid[neighbor.Y, neighbor.X] = CellState.Mine;
                madeDeduction = true;
            }

        foreach (var neighbor in neighbors.Where(n => _grid[n.Y, n.X] == CellState.Safe)) AddToFrontier(neighbor);

        return madeDeduction;
    }

    private bool PerformLogicalDeduction()
    {
        var overallProgress = false;
        bool madeProgress;

        do
        {
            madeProgress = false;

            for (var y = 0; y < _height; y++)
            for (var x = 0; x < _width; x++)
                if (DeduceFromCell(new Vector2I(x, y)))
                {
                    madeProgress = true;
                    overallProgress = true;
                }
        } while (madeProgress);

        return overallProgress;
    }

    private void FindAllSafeCells()
    {
        for (var y = 0; y < _height; y++)
        for (var x = 0; x < _width; x++)
            if (_grid[y, x] == CellState.Safe)
                AddToFrontier(new Vector2I(x, y));
    }

    public (List<Vector2I> path, bool reachedTarget) Solve(Vector2I start, Vector2I target)
    {
        _grid[start.Y, start.X] = CellState.Safe;
        _frontier.Clear();
        _path.Clear();
        AddToFrontier(start);

        while (_frontier.Count > 0)
        {
            var current = GetClosestFromFrontier(target);
            if (current == null) break;

            _path.Add(current);
            _grid[current.Y, current.X] = CellState.Visited;
            _mineCount[current.Y, current.X] = _oracle.GetMineCount(current);

            if (current.Equals(target)) return (_path.ToList(), true);

            PerformLogicalDeduction();

            if (_frontier.Count == 0) FindAllSafeCells();
        }

        return (_path.ToList(), false);
    }
}