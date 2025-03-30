using MinedOut.Core.Utilities;

namespace MinedOut.Core.Logic.Base;

public abstract class GameObject
{
    public delegate void PositionChangedHandler(PositionChangedEventArgs eventArgs);

    public readonly List<GameObject> Children = new();

    public readonly GameObject? Parent = null;

    private Vector2I _position = new();

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

    public virtual void Tick()
    {
    }

    public void AddChild(GameObject element)
    {
        Children.Add(element);
    }

    public void RemoveChild(GameObject element)
    {
        Children.Remove(element);
    }
}