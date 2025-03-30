using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;

namespace MinedOut.Core.Logic.World;

public class GameWorld
{
    public GameWorld(int width, int height)
    {
        ResetAndResize(width, height);
    }

    public int Height { get; private set; }

    public int Width { get; private set; }
    public Cell[,] Grid { get; private set; } = null!;

    public List<Entity> Entities { get; } = new();

    public event Action<Entity> EntityAdded;

    public void Reset()
    {
        Grid = new Cell[Width, Height];

        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            Grid[x, y] = x == 0 || x == Width - 1 || y == 0 || y == Height - 1
                ? CellsRegistry.Wall
                : CellsRegistry.Air;
    }

    public void AddEntity(Entity entity)
    {
        Entities.Add(entity);
        EntityAdded(entity);
    }

    public void ResetAndResize(int width, int height)
    {
        Width = width;
        Height = height;

        Reset();
    }
}