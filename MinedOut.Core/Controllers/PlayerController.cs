using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Controllers;

public class PlayerController : Controller
{
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;
    private readonly Player _player;

    public PlayerController(Player player, IGameInput gameInput, GameState gameState)
    {
        _player = player;
        _gameInput = gameInput;
        _gameState = gameState;

        _gameInput.KeyPressed += OnInputGained;
    }

    private void OnInputGained(KeyPressEventArgs eventArgs)
    {
        MovePlayer(
            new Vector2I(
                IGameInput.GetAxis(eventArgs.Key, Keys.Left, Keys.Right),
                IGameInput.GetAxis(eventArgs.Key, Keys.Up, Keys.Down)
            )
        );
    }

    private void MovePlayer(Vector2I movementVector)
    {
        var newPosition = _player.Position + movementVector;
        newPosition.X = Math.Clamp(newPosition.X, 0, _gameState.World.Width - 1);
        newPosition.Y = Math.Clamp(newPosition.Y, 0, _gameState.World.Height - 1);

        if (!_gameState.World.Grid[newPosition.X, newPosition.Y].IsPassable) return;

        foreach (var entity in _gameState.World.Entities)
            if (entity.Position.Equals(newPosition))
                return;

        _gameState.World.Grid[_player.Position.X, _player.Position.Y] = CellsRegistry.Path;

        _player.Position = newPosition;
    }
}