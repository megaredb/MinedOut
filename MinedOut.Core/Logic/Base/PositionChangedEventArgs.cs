using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.Base;

public class PositionChangedEventArgs(GameObject gameObject, Vector2I previousPosition) : EventArgs
{
    public readonly Vector2I PreviousPosition = previousPosition;
    public readonly GameObject Sender = gameObject;
}