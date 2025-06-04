using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Controllers;

public class LiveMineController : Controller
{
    private readonly GameState _gameState;
    private readonly LiveMine _liveMine;
    private Player? _targetPlayer;

    public LiveMineController(LiveMine liveMine, GameState gameState)
    {
        _liveMine = liveMine;
        _gameState = gameState;

        _gameState.World.EntityAdded += OnNewEntityAdded;

        foreach (var entity in _gameState.World.Entities) OnNewEntityAdded(entity);

        _gameState.GameOver += () => _liveMine.Drop();
        _gameState.NextLevel += () => _liveMine.Drop();

        new Thread(FollowPlayer).Start();
    }

    private bool ShouldUpdate()
    {
        return _gameState.Screen == Screen.Game && !_liveMine.IsDropped;
    }

    private void FollowPlayer()
    {
        while (ShouldUpdate())
        {
            Thread.Sleep(1000);

            if (!ShouldUpdate() || _gameState.ExitConfirmation) continue;

            if (_targetPlayer == null) continue;

            var path = AStar.FindPath(_liveMine.Position, _targetPlayer.Position, _gameState.World.Grid);

            Vector2I? nextPoint = null;

            if (path.Count > 0)
                nextPoint = path[1];

            if (nextPoint != null)
                _liveMine.Position = nextPoint;

            if (_liveMine.Position == _targetPlayer.Position)
            {
                _gameState.CallGameOver();
                return;
            }
        }
    }

    private void OnNewEntityAdded(Entity obj)
    {
        if (obj is Player player) _targetPlayer = player;
    }
}