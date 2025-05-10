using MinedOut.Core.Input;
using MinedOut.Core.Logic;

namespace MinedOut.Core.Controllers;

public class MenuController : Controller
{
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public MenuController(GameState gameState, IGameInput gameInput)
    {
        _gameState = gameState;
        _gameInput = gameInput;

        _gameInput.KeyPressed += OnInputGained;
    }

    private void OnInputGained(KeyPressEventArgs eventArgs)
    {
        if (_gameState.Screen != Screen.Menu) return;
        if (eventArgs.Key == Keys.Enter)
            _gameState.CallNextLevel();
    }
}