namespace MinedOut.Core.Utilities;

[Serializable]
public class Vector2I(int x = 0, int y = 0) : IEquatable<Vector2I>
{
    public int X = x;
    public int Y = y;

    public bool Equals(Vector2I? other)
    {
        if (other is null) return false;

        return other.X == X && other.Y == Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2I other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static Vector2I operator +(Vector2I a, Vector2I b)
    {
        return new Vector2I(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2I operator -(Vector2I a, Vector2I b)
    {
        return new Vector2I(a.X - b.X, a.Y - b.Y);
    }

    public static bool operator ==(Vector2I a, Vector2I b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Vector2I a, Vector2I b)
    {
        return !(a == b);
    }
}