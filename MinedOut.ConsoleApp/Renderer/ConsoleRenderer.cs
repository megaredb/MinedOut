using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Base;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Renderer;
using MinedOut.Core.Utilities;
using Path = MinedOut.Core.Logic.World.Cells.Path;

namespace MinedOut.ConsoleApp.Renderer;

public class ConsoleRenderer : IRenderer
{
    private readonly Dictionary<Type, ColoredChar> _chars = new()
    {
        { typeof(Wall), new ColoredChar('#', ConsoleColor.DarkGray) },
        { typeof(Air), new ColoredChar(' ') },
        { typeof(Mine), new ColoredChar('*', ConsoleColor.Red) },
        { typeof(Path), new ColoredChar('.', ConsoleColor.Gray) },
        { typeof(Player), new ColoredChar('@', ConsoleColor.Green) }
    };

    private readonly GameState _gameState;


    public ConsoleRenderer(GameState gameState)
    {
        _gameState = gameState;
        _gameState.World.EntityAdded += OnNewEntityAdded;
        _gameState.GameStopped += delegate
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.ResetColor();
        };
    }

    public static IRenderer CreateInstance(GameState gameState)
    {
        Console.CursorVisible = false;
        return new ConsoleRenderer(gameState);
    }

    public void Render()
    {
        if (!_gameState.IsRunning)
            return;

        Console.Clear();

        for (var y = 0; y < _gameState.World.Height; y++)
        {
            for (var x = 0; x < _gameState.World.Width; x++) UpdateAt(x, y);

            Console.WriteLine();
        }
    }

    private void OnNewEntityAdded(Entity entity)
    {
        entity.PositionChanged += OnEntityPositionChanged;
    }

    private void WriteColoredChar(ColoredChar coloredChar)
    {
        Console.ForegroundColor = coloredChar.Color;
        if (coloredChar.BackgroundColor != null)
            Console.BackgroundColor = (ConsoleColor)coloredChar.BackgroundColor;

        Console.Write(coloredChar.Value);
    }

    private void UpdateAt(int x, int y)
    {
        var cursorPosition = Console.GetCursorPosition();

        Console.SetCursorPosition(x, y);

        WriteColoredChar(GetCharForObjectAt(x, y));

        Console.SetCursorPosition(cursorPosition.Left, cursorPosition.Top);
    }

    private ColoredChar GetCharForObjectAt(int x, int y)
    {
        foreach (var entity in _gameState.World.Entities)
            if (entity.Position.Equals(new Vector2I(x, y)))
                return _chars[entity.GetType()];

        return _chars[_gameState.World.Grid[x, y].GetType()];
    }

    private void OnEntityPositionChanged(PositionChangedEventArgs eventArgs)
    {
        UpdateAt(eventArgs.PreviousPosition.X, eventArgs.PreviousPosition.Y);
        UpdateAt(eventArgs.Sender.Position.X, eventArgs.Sender.Position.Y);
    }
}