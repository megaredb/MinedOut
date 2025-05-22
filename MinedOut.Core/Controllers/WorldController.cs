using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;

namespace MinedOut.Core.Controllers;

public class WorldController : Controller
{
    private readonly IAudio _audio;
    private readonly List<Controller> _controllers = new();
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public WorldController(GameState gameState, IGameInput gameInput, IAudio audio)
    {
        _gameState = gameState;
        _gameInput = gameInput;
        _audio = audio;

        _gameState.World.EntityAdded += OnNewEntityAdded;
        var clearFunc = () => { _controllers.Clear(); };
        _gameState.GameOver += clearFunc;
        _gameState.NextLevel += clearFunc;

        foreach (var entity in _gameState.World.Entities) OnNewEntityAdded(entity);
    }

    private void OnNewEntityAdded(Entity entity)
    {
        if (entity is Player player) _controllers.Add(new PlayerController(player, _gameInput, _gameState, _audio));
        else if (entity is LiveMine liveMine) _controllers.Add(new LiveMineController(liveMine, _gameState));
    }
}