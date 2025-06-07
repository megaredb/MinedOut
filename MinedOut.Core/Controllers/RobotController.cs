using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Controllers;

public class RobotController : Controller
{
    private const int MoveDelayMs = 200;
    private readonly List<Vector2I> _currentPath;
    private readonly GameState _gameState;
    private readonly MinedOutSolver _solver;
    private readonly Vector2I _exitPosition;
    private int _pathIndex;
    private Robot? _robot;

    public RobotController(Robot robot, GameState gameState)
    {
        _robot = robot;
        _gameState = gameState;
        _currentPath = new List<Vector2I>();
        _pathIndex = 0;

        _exitPosition = FindExitPosition();

        _solver = new MinedOutSolver(gameState.World);

        var (path, reached) = _solver.Solve(robot.Position, _exitPosition);

        if (reached)
        {
            _currentPath = path;
            StartMoving();
        }
        else
        {
            _currentPath = path;
            if (_currentPath.Count > 0)
                StartMoving();
        }

        _gameState.GameOver += Drop;
        _gameState.NextLevel += Drop;
    }

    private Vector2I FindExitPosition()
    {
        for (var x = 0; x < _gameState.World.Width; x++)
        for (var y = 0; y < _gameState.World.Height; y++)
        {
            var cell = _gameState.World[x, y];
            if (cell is Exit) return new Vector2I(x, y);
        }

        return Vector2I.Zero;
    }

    private void StartMoving()
    {
        new Thread(() =>
        {
            while (_robot != null && _pathIndex < _currentPath.Count && _robot.Position != _exitPosition)
            {
                Thread.Sleep(MoveDelayMs);
                if (_robot == null) break;

                var nextMove = GetNextMove();

                if (nextMove == null)
                {
                    _gameState.World.Entities.Remove(_robot);
                    Drop();
                    break;
                }

                _robot.Position = nextMove;

                var targetCell = _gameState.World[_robot.Position.X, _robot.Position.Y];

                if (targetCell is not Air) continue;

                _gameState.World[_robot.Position.X, _robot.Position.Y] = CellsRegistry.Path;
            }
        }).Start();
    }

    private Vector2I? GetNextMove()
    {
        if (_pathIndex >= _currentPath.Count)
            return null;

        var nextMove = _currentPath[_pathIndex];
        _pathIndex++;
        return nextMove;
    }

    private void Drop()
    {
        if (_robot == null) return;

        _robot.Drop();
        _robot = null!;
    }
}