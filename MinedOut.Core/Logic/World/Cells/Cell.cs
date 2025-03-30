using MinedOut.Core.Logic.Base;

namespace MinedOut.Core.Logic.World.Cells;

public abstract class Cell : GameObject
{
    public virtual bool IsPassable { get; set; } = true;
}