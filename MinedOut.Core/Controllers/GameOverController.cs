using MinedOut.Core.Input;
using MinedOut.Core.Logic;

namespace MinedOut.Core.Controllers;

public class GameOverController : Controller
{
    private readonly IAudio _audio;
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public GameOverController(GameState gameState, IGameInput gameInput, IAudio audio)
    {
        _gameInput = gameInput;
        _gameState = gameState;
        _audio = audio;

        _gameState.GameOver += OnGameOver;
        _gameInput.KeyPressed += OnInputGained;
    }

    private void OnInputGained(KeyPressEventArgs eventArgs)
    {
        if (_gameState.Screen != Screen.GameOver) return;
        if (eventArgs.Key == Keys.Enter)
            _gameState.CallMenu();
    }

    private void OnGameOver()
    {
        _audio.PlayDieSound();
    }
}