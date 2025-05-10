using MinedOut.Core.Controllers;
using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Renderer;

namespace MinedOut.Core;

public class GameCore<TRenderer, TGameInput, TAudio>
    where TRenderer : IRenderer
    where TGameInput : IGameInput
    where TAudio : IAudio
{
    private readonly IAudio _audio;
    private readonly ExitController _exitController;
    private readonly IGameInput _gameInput;
    private readonly GameOverController _gameOverController;
    private readonly MenuController _menuController;
    private readonly IRenderer _renderer;
    private readonly WorldController _worldController;
    public readonly GameState GameState;

    public GameCore()
    {
        GameState = new GameState();

        _audio = TAudio.CreateInstance();
        _renderer = TRenderer.CreateInstance(GameState);
        _gameInput = TGameInput.CreateInstance(GameState);

        _exitController = new ExitController(GameState, _gameInput);
        _menuController = new MenuController(GameState, _gameInput);
        _gameOverController = new GameOverController(GameState, _gameInput, _audio);
        _worldController = new WorldController(GameState, _gameInput, _audio);
    }

    public void Run()
    {
        new Thread(() => { _renderer.Render(); }).Start();
        _gameInput.Run();
    }
}