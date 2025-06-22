using MinedOut.Core.Audio;
using MinedOut.Core.Controllers;
using MinedOut.Core.Input;
using MinedOut.Core.Logic;

namespace MinedOut.Core;

public class GameCore
{
    private readonly List<Controller> _controllers = new();
    public readonly IAudio Audio;
    public readonly IGameInput GameInput;
    public readonly GameState GameState;

    public GameCore(GameState gameState, IAudio audio, IGameInput gameInput)
    {
        GameState = gameState;

        Audio = audio;
        GameInput = gameInput;

        _controllers.Add(new ExitController(GameState, GameInput));
        _controllers.Add(new MenuController(GameState, GameInput));
        _controllers.Add(new GameOverController(GameState, GameInput, Audio));
        _controllers.Add(new WorldController(GameState, GameInput, Audio));
        _controllers.Add(new CheatsController(GameState, GameInput));

        Audio.PlayBackgroundMusic();
    }
}