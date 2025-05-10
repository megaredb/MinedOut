using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using Path = MinedOut.Core.Logic.World.Cells.Path;

namespace MinedOut.Core.Files;

public static class DataConverter
{
    public static EntityData ToEntityData(Entity entity)
    {
        return new EntityData
        {
            EntityType = entity switch
            {
                Player => EntityType.Player,
                _ => throw new NotImplementedException("Unable to serialize unknown entity type.")
            },
            Position = entity.Position
        };
    }

    public static Entity ToEntity(EntityData entityData)
    {
        return entityData.EntityType switch
        {
            EntityType.Player => new Player(entityData.Position),
            _ => throw new NotImplementedException("Unable to deserialize unknown entity type.")
        };
    }

    public static int ToInt(Cell cell)
    {
        return cell switch
        {
            Mine => 1,
            Path => 2,
            Wall => 3,
            Exit => 4,
            _ => 0
        };
    }

    public static Cell ToCell(int value)
    {
        return value switch
        {
            1 => CellsRegistry.Mine,
            2 => CellsRegistry.Path,
            3 => CellsRegistry.Wall,
            _ => CellsRegistry.Air
        };
    }
}