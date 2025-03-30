using MinedOut.Core.Logic.World;

namespace MinedOut.Core.Logic;

public class GameState
{
    public GameState()
    {
        Restart();
    }

    public bool IsRunning { get; private set; } = true;
    public GameWorld World { get; private set; } = null!;
    public event Action? GameStopped;

    public void Restart()
    {
        int width = 32, height = 32;

        World = new GameWorld(width, height);
    }

    public void Stop()
    {
        IsRunning = false;
        GameStopped?.Invoke();
    }
}