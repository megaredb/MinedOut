using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Controllers;

public class PlayerController : Controller
{
    private readonly IAudio _audio;
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;
    private readonly Player _player;

    public PlayerController(Player player, IGameInput gameInput, GameState gameState, IAudio audio)
    {
        _player = player;
        _gameInput = gameInput;
        _gameState = gameState;
        _audio = audio;

        _gameInput.KeyPressed += OnInputGained;

        var clearFunc = () => { _gameInput.KeyPressed -= OnInputGained; };
        _gameState.GameOver += clearFunc;
        _gameState.NextLevel += clearFunc;
    }

    private void OnInputGained(KeyPressEventArgs eventArgs)
    {
        if (_gameState.Screen != Screen.Game) return;

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

        var cell = _gameState.World[newPosition.X, newPosition.Y];
        if (!cell.IsPassable) return;

        if (cell.PlayerInteract(_gameState)) return;

        var entity = _gameState.World.Entities.FirstOrDefault(e => e.Position.Equals(newPosition));
        if (entity != null)
            if (entity.PlayerInteract(_gameState))
                return;

        _gameState.World[_player.Position.X, _player.Position.Y] = CellsRegistry.Path;

        _player.Position = newPosition;
        _audio.PlayMoveSound();
    }
}