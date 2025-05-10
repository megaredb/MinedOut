using MinedOut.Core.Logic.World;
using MinedOut.Core.Logic.World.Cells;

namespace MinedOut.Core.Files;

[Serializable]
public class LevelData
{
    public required string Name { get; set; }

    public required int Width { get; set; }
    public required int Height { get; set; }

    public required int[] Grid { get; set; } // this field contains ids of cells
    public required List<EntityData> Entities { get; set; }

    public static implicit operator LevelData(GameWorld value)
    {
        var grid = new int[value.Width * value.Height];

        for (var x = 0; x < value.Width; x++)
        for (var y = 0; y < value.Height; y++)
            grid[x * value.Width + y] = DataConverter.ToInt(value[x, y]);

        return new LevelData
        {
            Name = value.Name,
            Width = value.Width,
            Height = value.Height,
            Grid = grid,
            Entities = value.Entities.Select(
                DataConverter.ToEntityData
            ).ToList()
        };
    }

    public static implicit operator GameWorld(LevelData value)
    {
        var entities = value.Entities.Select(DataConverter.ToEntity).ToList();
        var grid = new Cell[value.Width, value.Height];

        for (var x = 0; x < value.Width; x++)
        for (var y = 0; y < value.Height; y++)
            grid[x, y] = DataConverter.ToCell(value.Grid[x * value.Width + y]);

        return new GameWorld(
            value.Width,
            value.Height,
            value.Name,
            grid,
            entities
        );
    }
}