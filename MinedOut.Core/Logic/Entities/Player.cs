using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.Entities;

public class Player : Entity
{
    public Player(Vector2I position)
    {
        Position = position;
    }
}