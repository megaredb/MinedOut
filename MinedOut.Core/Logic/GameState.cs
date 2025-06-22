using MinedOut.Core.Audio;
using MinedOut.Core.Logic.World;

namespace MinedOut.Core.Logic;

public class GameState
{
    private bool _exitConfirmation;
    private int _score;

    private Screen _screen = Screen.Menu;
    public IAudio? Audio;

    public GameState()
    {
        NextLevel += OnNextLevel;
        OnNextLevel();
    }

    public bool ExitConfirmation
    {
        get => _exitConfirmation;
        set
        {
            _exitConfirmation = value;
            ExitConfirmationOpened?.Invoke();
        }
    }

    public bool IsRunning { get; private set; } = true;
    public GameWorld World { get; } = new(32, 32);

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            ScoreChanged?.Invoke();
        }
    }

    public Screen Screen
    {
        get => _screen;
        private set
        {
            _screen = value;
            ScreenChanged?.Invoke();
        }
    }

    public void CallMenu()
    {
        ReturnedToMenu?.Invoke();
        Screen = Screen.Menu;
    }

    public void CallGameOver()
    {
        if (Screen != Screen.Game) return;

        GameOver?.Invoke();
        Screen = Screen.GameOver;
    }

    public void CallNextLevel()
    {
        if (Screen == Screen.Menu) Score = 0;
        if (Audio != null) Audio.PlayNextLevelSound();
        Screen = Screen.Game;
        Score += 1;
        NextLevel?.Invoke();
    }

    public event Action? GameStopped;
    public event Action? ReturnedToMenu;
    public event Action? GameOver;
    public event Action? NextLevel;
    public event Action? ScoreChanged;
    public event Action? ScreenChanged;
    public event Action? ExitConfirmationOpened;
    public event Action? RedrawCalled;

    public void Redraw()
    {
        RedrawCalled?.Invoke();
    }

    private void OnNextLevel()
    {
        World.CopyDataFrom(new WorldGenerator().GenerateWorld(32, 32));
    }

    public void Stop()
    {
        IsRunning = false;
        CallMenu();
        GameStopped?.Invoke();
    }
}