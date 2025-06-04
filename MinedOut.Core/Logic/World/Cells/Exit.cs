namespace MinedOut.Core.Logic.World.Cells;

public class Exit : Cell
{
    public override bool PlayerInteract(GameState gameState)
    {
        gameState.CallNextLevel();
        return true;
    }
}