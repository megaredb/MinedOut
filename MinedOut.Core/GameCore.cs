using MinedOut.Core.Controllers;
using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Renderer;
using MinedOut.Core.Utilities;

namespace MinedOut.Core;

public class GameCore<TRenderer, TGameInput>
    where TRenderer : IRenderer
    where TGameInput : IGameInput
{
    private readonly IGameInput _gameInput;
    private readonly IRenderer _renderer;
    private readonly WorldController _worldController;
    public readonly GameState GameState;

    public GameCore()
    {
        GameState = new GameState();

        _renderer = TRenderer.CreateInstance(GameState);
        _gameInput = TGameInput.CreateInstance(GameState);

        _worldController = new WorldController(GameState, _gameInput);
    }

    public void Run()
    {
        new Thread(() => { _renderer.Render(); }).Start();
        GameState.World.AddEntity(new Player(new Vector2I(GameState.World.Height / 2, GameState.World.Width / 2)));
        GameState.World.AddEntity(new Player(new Vector2I(GameState.World.Height / 2, 5)));
        _gameInput.Run();
    }
}