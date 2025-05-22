using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinedOut.Core;
using MinedOut.Core.Logic;
using MinedOut.DesktopApp.Audio;
using MinedOut.DesktopApp.Input;

namespace MinedOut.DesktopApp;

public class App : Game
{
    private readonly DesktopAudio _audio;
    private readonly DesktopGameInput _gameInput;
    private GameCore _gameCore;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public App()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        var gameState = new GameState();

        gameState.GameStopped += Exit;

        _audio = new DesktopAudio();
        _gameInput = new DesktopGameInput();

        _gameCore = new GameCore(gameState, _audio, _gameInput);
    }

    protected override void Initialize()
    {
        _gameInput.Initialize();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _audio.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        _gameInput.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}