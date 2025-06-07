using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.World;

public class WorldGenerator
{
    private readonly int _pathDownChance = 15;
    private readonly int _pathWidth = 2;
    private readonly Random _random = new();
    private int _mineChance = 8;

    private GameWorld MineLayer(GameWorld world, int difficulty)
    {
        var dangerZones = new List<Vector2I>();

        _mineChance = 5 + difficulty * 2;

        for (var i = 0; i < difficulty * 2; i++)
        {
            var zoneY = _random.Next(1, world.Height - 2);
            var zoneX = _random.Next(1, world.Width - 2);
            var zoneWidth = _random.Next(3, 5 + difficulty);
            var zoneHeight = _random.Next(2, 3 + difficulty);

            for (var x = Math.Max(1, zoneX); x < Math.Min(world.Width - 2, zoneX + zoneWidth); x++)
            for (var y = Math.Max(1, zoneY); y < Math.Min(world.Height - 2, zoneY + zoneHeight); y++)
                if (_random.Next(100) < 70)
                    dangerZones.Add(new Vector2I(x, y));
        }

        for (var x = 1; x < world.Width - 1; x++)
        for (var y = 1; y < world.Height - 1; y++)
        {
            var pos = new Vector2I(x, y);
            var inDangerZone = dangerZones.Contains(pos);
            var mineChance = inDangerZone ? _mineChance * 2 : _mineChance / 2;

            if (_random.Next(100) < mineChance) world[x, y] = CellsRegistry.Mine;
        }

        return world;
    }

    private GameWorld WallLayer(GameWorld world)
    {
        var wallClusters = new List<List<Vector2I>>();

        var clusterCount = Math.Min(6, 3 + world.Width * world.Height / 400);

        for (var i = 0; i < clusterCount; i++)
        {
            var patternType = _random.Next(0, 4);
            var cluster = new List<Vector2I>();

            var startX = _random.Next(1, world.Width - 2);
            var startY = _random.Next(1, world.Height - 2);

            switch (patternType)
            {
                case 0:
                    var length = _random.Next(3, 8);
                    for (var x = 0; x < length; x++)
                    {
                        var posX = Math.Clamp(startX + x, 1, world.Width - 2);
                        cluster.Add(new Vector2I(posX, startY));

                        if (_random.Next(100) < 30 && startY > 1)
                            cluster.Add(new Vector2I(posX, startY - 1));
                        if (_random.Next(100) < 30 && startY < world.Height - 2)
                            cluster.Add(new Vector2I(posX, startY + 1));
                    }

                    break;

                case 1:
                    var height = _random.Next(3, 6);
                    for (var y = 0; y < height; y++)
                        if (_random.Next(100) < 80)
                        {
                            var posY = Math.Clamp(startY + y, 1, world.Height - 2);
                            cluster.Add(new Vector2I(startX, posY));

                            if (_random.Next(100) < 40 && startX > 1)
                                cluster.Add(new Vector2I(startX - 1, posY));
                            if (_random.Next(100) < 40 && startX < world.Width - 2)
                                cluster.Add(new Vector2I(startX + 1, posY));
                        }

                    break;

                case 2:
                    var roomWidth = _random.Next(3, 6);
                    var roomHeight = _random.Next(3, 5);

                    for (var x = 0; x < roomWidth; x++)
                    for (var y = 0; y < roomHeight; y++)
                        if (x == 0 || x == roomWidth - 1 || y == 0 || y == roomHeight - 1)
                        {
                            var posX = Math.Clamp(startX + x, 1, world.Width - 2);
                            var posY = Math.Clamp(startY + y, 1, world.Height - 2);
                            cluster.Add(new Vector2I(posX, posY));
                        }

                    break;

                case 3:
                    var diagLength = _random.Next(4, 8);
                    var xDir = _random.Next(2) == 0 ? 1 : -1;
                    var yDir = _random.Next(2) == 0 ? 1 : -1;

                    for (var d = 0; d < diagLength; d++)
                    {
                        var posX = Math.Clamp(startX + d * xDir, 1, world.Width - 2);
                        var posY = Math.Clamp(startY + d * yDir, 1, world.Height - 2);

                        cluster.Add(new Vector2I(posX, posY));

                        if (_random.Next(100) < 50)
                        {
                            var offsetX = _random.Next(2) == 0 ? 1 : -1;
                            var offsetY = _random.Next(2) == 0 ? 1 : -1;
                            var thickX = Math.Clamp(posX + offsetX, 1, world.Width - 2);
                            var thickY = Math.Clamp(posY + offsetY, 1, world.Height - 2);
                            cluster.Add(new Vector2I(thickX, thickY));
                        }
                    }

                    break;
            }

            wallClusters.Add(cluster);
        }

        foreach (var cluster in wallClusters)
        foreach (var point in cluster)
        {
            if (world[point.X, point.Y] is not Air)
                continue;

            world[point.X, point.Y] = CellsRegistry.Wall;
        }

        for (var i = 0; i < world.Width * world.Height / 20; i++)
        {
            var x = _random.Next(1, world.Width - 1);
            var y = _random.Next(1, world.Height - 1);

            if (world[x, y] is Air && CountAdjacentWalls(world, x, y) > 0) world[x, y] = CellsRegistry.Wall;
        }

        return world;
    }

    private int CountAdjacentWalls(GameWorld world, int x, int y)
    {
        var count = 0;
        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0) continue;
            var nx = x + dx;
            var ny = y + dy;
            if (nx >= 0 && nx < world.Width && ny >= 0 && ny < world.Height && world[nx, ny] is Wall)
                count++;
        }

        return count;
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
        var lastDirection = Vector2I.Zero;
        var stepsInSameDirection = 0;
        const int maxStepsSameDirection = 3;

        while (cursor.Y >= 1)
        {
            var currentWidth = _pathWidth + _random.Next(-1, 2);
            currentWidth = Math.Max(1, Math.Min(3, currentWidth));

            var left = cursor.X - currentWidth / 2;
            var right = left + currentWidth - 1;

            left = Math.Max(1, left);
            right = Math.Min(world.Width - 2, right);

            for (var x = left; x <= right; x++)
            {
                world[x, cursor.Y] = CellsRegistry.Air;

                if (_random.Next(100) < 20 && cursor.Y > 1) world[x, cursor.Y - 1] = CellsRegistry.Air;
            }

            Vector2I direction;

            if (stepsInSameDirection > maxStepsSameDirection)
            {
                direction = new Vector2I(
                    _random.Next(-1, 2),
                    _random.Next(100) < 70 ? -1 : 0
                );
                stepsInSameDirection = 0;
            }
            else
            {
                var rand = _random.Next(100);
                if (rand < 60)
                    direction = new Vector2I(0, -1);
                else if (rand < 80)
                    direction = new Vector2I(-1);
                else
                    direction = new Vector2I(1);

                if (_random.Next(100) < _pathDownChance / 2)
                    direction.Y = 1;
            }

            if (direction == lastDirection)
                stepsInSameDirection++;
            else
                stepsInSameDirection = 0;

            lastDirection = direction;

            cursor = (cursor + direction).Clamp(
                world.Width - 2, world.Height - 2
            );
        }

        for (var x = -1; x <= 1; x++)
        {
            var exitX = Math.Clamp(cursor.X + x, 0, world.Width - 1);

            world[exitX, 0] = CellsRegistry.Exit;
            world[exitX, 1] = CellsRegistry.Air;
        }

        for (var x = 1; x <= world.Width - 2; x++) world[x, world.Height - 2] = CellsRegistry.Air;

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

    private void PlaceBonusCoin(GameWorld world)
    {
        var attempts = 0;
        const int maxAttempts = 50;

        while (attempts < maxAttempts)
        {
            var x = _random.Next(2, world.Width - 2);
            var y = _random.Next(2, world.Height / 2);

            if (world[x, y] is Air &&
                !world.Entities.Any(e => e.Position.X == x && e.Position.Y == y) &&
                !IsNearPlayer(world, x, y, 10))
            {
                var mineCount = CountAdjacentMines(world, x, y);

                if (mineCount > 0 && mineCount <= 3)
                {
                    world[x, y] = CellsRegistry.BonusCoin;
                    return;
                }
            }

            attempts++;
        }

        var safeX = _random.Next(2, world.Width - 2);
        var safeY = _random.Next(2, world.Height / 2);
        if (world[safeX, safeY] is Air) world[safeX, safeY] = CellsRegistry.BonusCoin;
    }

    private bool IsNearPlayer(GameWorld world, int x, int y, int minDistance)
    {
        foreach (var entity in world.Entities.OfType<Player>())
        {
            var dx = Math.Abs(entity.Position.X - x);
            var dy = Math.Abs(entity.Position.Y - y);
            if (dx < minDistance && dy < minDistance)
                return true;
        }

        return false;
    }

    private int CountAdjacentMines(GameWorld world, int x, int y)
    {
        var count = 0;

        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0) continue;

            var nx = x + dx;
            var ny = y + dy;

            if (nx >= 0 && nx < world.Width && ny >= 0 && ny < world.Height)
                if (world[nx, ny] is Mine)
                    count++;
        }

        return count;
    }

    public GameWorld GenerateWorld(int width, int height, int? difficulty = null)
    {
        var world = new GameWorld(width, height);
        world.Difficulty = difficulty ?? _random.Next(1, 4);

        world = MineLayer(world, world.Difficulty);
        world = WallLayer(world);
        world = PlacePlayer(world);

        world = world.Entities.OfType<Player>().Aggregate(
            world,
            FreePlayerPath
        );


        world = PlaceLiveMine(world);

        PlaceBonusCoin(world);

        return world;
    }
}