namespace MinedOut.Core.Logic.World.Cells;

public class Mine : Cell
{
    public override bool PlayerInteract(GameState gameState)
    {
        gameState.CallGameOver();
        return true;
    }
}