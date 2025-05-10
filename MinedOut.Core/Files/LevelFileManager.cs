using System.Text.Json;
using MinedOut.Core.Logic.World;

namespace MinedOut.Core.Files;

public class LevelFileManager
{
    private const string LevelFolder = "./Levels";

    public static GameWorld? LoadLevel(string name)
    {
        using var stream = File.OpenRead($"{LevelFolder}/{name}.json");

        var levelData = JsonSerializer.Deserialize<LevelData>(stream);

        if (levelData == null)
            return null;

        return (GameWorld)levelData;
    }

    public static void SaveLevel(GameWorld world)
    {
        if (!LevelFolderExists())
            Directory.CreateDirectory(LevelFolder);

        using var stream = File.Create($"{LevelFolder}/{world.Name}.json");

        var levelData = (LevelData)world;

        JsonSerializer.Serialize(stream, levelData);
    }

    private static bool LevelFolderExists()
    {
        return Directory.Exists(LevelFolder);
    }
}