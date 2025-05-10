using MinedOut.Core.Utilities;

namespace MinedOut.Core.Files;

[Serializable]
public class EntityData
{
    public required EntityType EntityType { get; set; }
    public required Vector2I Position { get; set; }
}