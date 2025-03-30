using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;

namespace MinedOut.Core.Controllers;

public class WorldController : Controller
{
    private readonly List<Controller> _controllers = new();
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public WorldController(GameState gameState, IGameInput gameInput)
    {
        _gameState = gameState;
        _gameInput = gameInput;

        _gameState.World.EntityAdded += OnNewEntityAdded;
    }

    private void OnNewEntityAdded(Entity entity)
    {
        if (entity is Player player) _controllers.Add(new PlayerController(player, _gameInput, _gameState));
    }
}