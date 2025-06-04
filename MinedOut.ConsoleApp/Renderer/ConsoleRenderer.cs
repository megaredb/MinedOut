using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Base;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;
using Path = MinedOut.Core.Logic.World.Cells.Path;

namespace MinedOut.ConsoleApp.Renderer;

public class ConsoleRenderer
{
    private readonly Dictionary<Type, ColoredChar> _chars = new()
    {
        { typeof(Wall), new ColoredChar('#', ConsoleColor.DarkGray) },
        { typeof(Air), new ColoredChar(' ') },
        { typeof(Mine), new ColoredChar('*', ConsoleColor.Blue) },
        { typeof(Path), new ColoredChar('.', ConsoleColor.Gray) },
        { typeof(Player), new ColoredChar('@', ConsoleColor.Green) },
        { typeof(Exit), new ColoredChar('$', ConsoleColor.Blue) },
        { typeof(LiveMine), new ColoredChar('*', ConsoleColor.Red) }
    };

    private readonly GameState _gameState;

    private Vector2I _lastWindowSize = GetWindowSize();

    public ConsoleRenderer(GameState gameState)
    {
        Console.CursorVisible = false;

        _gameState = gameState;
        _gameState.World.EntityAdded += OnNewEntityAdded;
        _gameState.ScreenChanged += Render;
        _gameState.ExitConfirmationOpened += Render;
        _gameState.NextLevel += Render;

        _gameState.RedrawCalled += Render;

        foreach (var entity in _gameState.World.Entities)
            entity.PositionChanged += OnEntityPositionChanged;

        _gameState.ScoreChanged += OnScoreChanged;

        _gameState.GameStopped += delegate
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.ResetColor();
        };
    }

    private Vector2I Offset
    {
        get
        {
            var windowSize = GetWindowSize();

            return new Vector2I(
                (windowSize.X - _gameState.World.Width) / 2,
                (windowSize.Y - _gameState.World.Height) / 2);
        }
    }

    public void Render()
    {
        if (!_gameState.IsRunning)
            return;

        Console.Clear();

        _lastWindowSize = GetWindowSize();
        var xCenter = _lastWindowSize.X / 2;
        var yCenter = _lastWindowSize.Y / 2;

        if (_gameState.Screen == Screen.Game && !_gameState.ExitConfirmation)
        {
            for (var y = 0; y < _gameState.World.Height; y++)
            {
                for (var x = 0; x < _gameState.World.Width; x++) UpdateAt(x, y);

                Console.WriteLine();
            }

            OnScoreChanged();
            return;
        }


        string[] lines;

        if (_gameState.ExitConfirmation)
            lines = new[]
            {
                "Are you sure you want to exit?",
                "Press Enter to confirm or Escape to cancel"
            };
        else if (_gameState.Screen == Screen.Menu)
            lines = new[]
            {
                "MinedOut",
                "",
                "Press Enter to start",
                "Press Escape to exit"
            };
        else
            lines = new[]
            {
                "Game Over",
                "",
                "Press Enter to exit to menu",
                "Your score was: " + _gameState.Score
            };

        yCenter -= lines.Length / 2;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            WriteAt(xCenter - line.Length / 2, yCenter + i, line);
        }
    }

    private void OnScoreChanged()
    {
        if (_gameState.Screen != Screen.Game) return;

        var coloredChars = ColoredChar.FromString($"Score: {_gameState.Score}", ConsoleColor.Yellow);

        for (var i = 0; i < coloredChars.Count; i++) WriteAt(Offset.X + i, Offset.Y - 1, coloredChars[i]);
    }

    private void OnNewEntityAdded(Entity entity)
    {
        entity.PositionChanged += OnEntityPositionChanged;
    }

    private static Vector2I GetWindowSize()
    {
        return new Vector2I(Console.WindowWidth, Console.WindowHeight);
    }

    private bool IsWindowSizeChanged()
    {
        return GetWindowSize() != _lastWindowSize;
    }

    private void WriteColoredChar(ColoredChar coloredChar)
    {
        Console.ForegroundColor = coloredChar.Color;
        if (coloredChar.BackgroundColor != null)
            Console.BackgroundColor = (ConsoleColor)coloredChar.BackgroundColor;

        Console.Write(coloredChar.Value);
    }

    private void WriteAt(int x, int y, ColoredChar coloredChar)
    {
        Console.SetCursorPosition(x, y);

        WriteColoredChar(coloredChar);
    }

    private void WriteAt(int x, int y, string text, ConsoleColor color = ConsoleColor.White,
        ConsoleColor? backgroundColor = null)
    {
        var coloredChars = ColoredChar.FromString(text, color, backgroundColor);

        for (var i = 0; i < coloredChars.Count; i++) WriteAt(x + i, y, coloredChars[i]);
    }

    private void UpdateAt(int x, int y)
    {
        if (IsWindowSizeChanged()) Render();

        WriteAt(x + Offset.X, y + Offset.Y, GetCharForObjectAt(x, y));
    }

    private ColoredChar GetCharForObjectAt(int x, int y)
    {
        foreach (var entity in _gameState.World.Entities)
            if (entity.Position.Equals(new Vector2I(x, y)))
            {
                if (entity is Player player)
                {
                    var playerChar = _chars[typeof(Player)];
                    var count = 0;
                    for (var dx = -1; dx <= 1; dx++)
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (((dy + 1) * 3 + dx + 1) % 2 == 0) continue;
                        var position = player.Position + new Vector2I(dx, dy);
                        position.X = Math.Clamp(position.X, 0, _gameState.World.Width - 1);
                        position.Y = Math.Clamp(position.Y, 0, _gameState.World.Height - 1);

                        if (_gameState.World[position.X, position.Y] is Mine)
                            count++;
                    }

                    playerChar.Value = count.ToString()[0];
                    playerChar.Color = ConsoleColor.Green + count;

                    return playerChar;
                }

                return _chars[entity.GetType()];
            }

        return _chars[_gameState.World[x, y].GetType()];
    }

    private void OnEntityPositionChanged(PositionChangedEventArgs eventArgs)
    {
        UpdateAt(eventArgs.PreviousPosition.X, eventArgs.PreviousPosition.Y);
        UpdateAt(eventArgs.Sender.Position.X, eventArgs.Sender.Position.Y);
    }
}