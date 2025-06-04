using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.Base;

public abstract class GameObject
{
    public delegate void PositionChangedHandler(PositionChangedEventArgs eventArgs);

    private Vector2I _position = new();
    public bool IsDropped { get; private set; }

    public Vector2I Position
    {
        get => _position;
        set
        {
            if (_position.Equals(value)) return;
            var prevPosition = new Vector2I(_position.X, _position.Y);
            _position = value;
            OnPositionChanged(new PositionChangedEventArgs(this, prevPosition));
        }
    }

    protected virtual void OnPositionChanged(PositionChangedEventArgs eventArgs)
    {
        PositionChanged?.Invoke(eventArgs);
    }

    public event PositionChangedHandler? PositionChanged;

    public void Drop()
    {
        IsDropped = true;
        PositionChanged = null;
    }

    public virtual bool PlayerInteract(GameState gameState)
    {
        return false;
    }
}