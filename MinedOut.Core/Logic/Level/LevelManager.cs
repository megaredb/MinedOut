using MinedOut.Core.Logic.World;

namespace MinedOut.Core.Logic.Level;

public abstract class LevelManager
{
    public abstract GameWorld NextWorld();
}