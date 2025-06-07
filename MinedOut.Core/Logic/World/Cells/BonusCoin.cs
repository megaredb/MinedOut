using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.World.Cells;

public class BonusCoin : Cell
{
    public override bool PlayerInteract(GameState gameState)
    {
        var robot = new Robot(new Vector2I(gameState.World.Width / 2, gameState.World.Height - 1));
        gameState.World.AddEntity(robot);

        return false;
    }
}