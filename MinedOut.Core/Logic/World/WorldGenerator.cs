using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.World;

public class WorldGenerator
{
    private readonly int _mineChance = 8;
    private readonly int _pathDownChance = 10;
    private readonly Random _random = new();

    private GameWorld MineLayer(GameWorld world)
    {
        for (var x = 1; x < world.Width - 1; x++)
        for (var y = 1; y < world.Height - 1; y++)
            if (_random.Next(0, 100) < _mineChance)
                world[x, y] = CellsRegistry.Mine;

        return world;
    }

    private GameWorld WallLayer(GameWorld world)
    {
        var wallPoints = new List<Vector2I>();

        for (var x = 0; x < world.Width; x++)
        {
            if (_random.Next(0, 2) == 0) continue;
            var wallY = _random.Next(1, world.Height - 1);
            wallPoints.Add(new Vector2I(x, wallY));
        }

        foreach (var wallPoint in wallPoints)
            if (wallPoint.X > 0 && wallPoint.X < world.Width - 1)
            {
                world[wallPoint.X, wallPoint.Y] = CellsRegistry.Wall;
                world[wallPoint.X + 1, wallPoint.Y] = CellsRegistry.Wall;
                world[wallPoint.X - 1, wallPoint.Y] = CellsRegistry.Wall;
                if (wallPoint.Y > 0)
                    world[wallPoint.X, wallPoint.Y - 1] = CellsRegistry.Wall;
                if (wallPoint.Y < world.Height - 1)
                    world[wallPoint.X, wallPoint.Y + 1] = CellsRegistry.Wall;
            }

        return world;
    }

    private GameWorld PlacePlayer(GameWorld world)
    {
        var player = new Player(new Vector2I(world.Width / 2, world.Height - 1));
        world.AddEntity(player);

        for (var x = player.Position.X - 1; x <= player.Position.X + 1; x++)
        for (var y = player.Position.Y - 1; y <= player.Position.Y + 1; y++)
            if (x >= 0 && x < world.Width && y >= 0 && y < world.Height)
                world[x, y] = CellsRegistry.Air;

        return world;
    }

    private GameWorld FreePlayerPath(GameWorld world, Player player)
    {
        var cursor = new Vector2I(player.Position.X, player.Position.Y);

        while (cursor.Y >= 1)
        {
            for (var x = Math.Max(cursor.X - 1, 1); x <= Math.Min(cursor.X + 1, world.Width - 2); x++)
            for (var y = Math.Max(cursor.Y - 1, 1); y <= Math.Min(cursor.Y + 1, world.Height - 2); y++)
            {
                if (Math.Abs(x - cursor.X) + Math.Abs(y - cursor.Y) > 1)
                    continue;

                world[x, y] = CellsRegistry.Air;
            }

            var direction = new Vector2I(_random.Next(-1, 2), _random.Next(-1, 1));

            if (_random.Next(100) < _pathDownChance) direction.Y = 1;

            cursor += direction;
            cursor = new Vector2I(
                Math.Clamp(cursor.X, 0, world.Width - 2),
                Math.Clamp(cursor.Y, 0, world.Height - 2)
            );
        }

        for (var x = -1; x <= 1; x++)
            world[Math.Clamp(cursor.X + x, 0, world.Width - 1), Math.Clamp(cursor.Y, 0, world.Height - 1)] =
                CellsRegistry.Exit;

        return world;
    }

    private GameWorld PlaceLiveMine(GameWorld world)
    {
        List<Vector2I> minesPositions = new();

        for (var x = 1; x < world.Width - 1; x++)
        for (var y = 1; y < world.Height - 1; y++)
            if (world[x, y] is Mine)
                minesPositions.Add(new Vector2I(x, y));

        var liveMineCount = 0;

        do
        {
            var liveMine = new LiveMine(minesPositions[_random.Next(0, Math.Abs(minesPositions.Count - 1) / 2)]);
            world.AddEntity(liveMine);
            liveMineCount++;
        } while (_random.Next(0, 5) < 1 && liveMineCount < 1);

        return world;
    }

    public GameWorld GenerateWorld(int width, int height)
    {
        var world = new GameWorld(width, height);

        world = MineLayer(world);
        world = WallLayer(world);
        world = PlacePlayer(world);
        world = PlaceLiveMine(world);

        foreach (var entity in world.Entities)
        {
            if (entity is not Player player) continue;
            world = FreePlayerPath(world, player);
        }

        return world;
    }
}