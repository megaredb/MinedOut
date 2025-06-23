namespace MinedOut.Core.Utilities;

public class Vector2I(int x = 0, int y = 0) : IEquatable<Vector2I>
{
    public static readonly Vector2I Zero = new();

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

    public static bool operator ==(Vector2I? a, Vector2I? b)
    {
        return a is null ? b is null : a.Equals(b);
    }

    public static bool operator !=(Vector2I? a, Vector2I? b)
    {
        return !(a == b);
    }

    public Vector2I Clamp(int minX, int maxX, int minY, int maxY)
    {
        return new Vector2I(
            Math.Clamp(X, minX, maxX),
            Math.Clamp(Y, minY, maxY)
        );
    }

    public Vector2I Clamp(int x, int y)
    {
        return new Vector2I(
            Math.Clamp(X, 0, x),
            Math.Clamp(Y, 0, y)
        );
    }
}