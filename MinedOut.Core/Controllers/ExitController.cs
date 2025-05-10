using MinedOut.Core.Input;
using MinedOut.Core.Logic;

namespace MinedOut.Core.Controllers;

public class ExitController : Controller
{
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public ExitController(GameState gameState, IGameInput gameInput)
    {
        _gameState = gameState;
        _gameInput = gameInput;

        _gameInput.KeyPressed += OnInputGained;
    }

    public void OnInputGained(KeyPressEventArgs eventArgs)
    {
        if (eventArgs.Key == Keys.Escape)
            _gameState.ExitConfirmation = !_gameState.ExitConfirmation;

        if (eventArgs.Key == Keys.Enter && _gameState.ExitConfirmation)
            _gameState.Stop();
    }
}