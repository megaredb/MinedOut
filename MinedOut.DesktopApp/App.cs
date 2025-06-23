using Microsoft.Xna.Framework;
using MinedOut.Core;
using MinedOut.Core.Logic;
using MinedOut.DesktopApp.Audio;
using MinedOut.DesktopApp.Input;
using MinedOut.DesktopApp.Renderer;

namespace MinedOut.DesktopApp;

public class App : Game
{
    private readonly DesktopAudio _audio;
    private readonly GameCore _gameCore;
    private readonly DesktopGameInput _gameInput;
    private readonly GraphicsDeviceManager _graphics;
    private DesktopRenderer? _renderer;

    public App()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        var gameState = new GameState();

        gameState.GameStopped += Exit;

        _audio = new DesktopAudio();
        gameState.Audio = _audio;
        _gameInput = new DesktopGameInput();

        _gameCore = new GameCore(gameState, _audio, _gameInput);

        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 1024;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        _gameInput.Initialize();

        base.Initialize();
        _gameCore.Audio.PlayBackgroundMusic();
    }

    protected override void LoadContent()
    {
        _renderer = new DesktopRenderer(_gameCore.GameState, GraphicsDevice);
        _renderer.LoadContent(this);
        _audio.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        _gameInput.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _renderer?.Render();

        base.Draw(gameTime);
    }
}