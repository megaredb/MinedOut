namespace MinedOut.Core.Logic.World.Cells;

public class Wall : Cell
{
    public override bool IsPassable { get; set; } = false;
}