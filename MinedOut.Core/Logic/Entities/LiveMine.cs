using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.Entities;

public class LiveMine : Entity
{
    public LiveMine(Vector2I position)
    {
        Position = position;
    }

    public override bool PlayerInteract(GameState gameState)
    {
        gameState.CallGameOver();
        return true;
    }
}