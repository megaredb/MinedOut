using MinedOut.Core.Input;
using MinedOut.Core.Logic;

namespace MinedOut.ConsoleApp.Input;

public class ConsoleGameInput(GameState gameState) : IGameInput
{
    private readonly Dictionary<ConsoleKey, Keys> _keysConversionMap =
        new()
        {
            { ConsoleKey.W, Keys.Up },
            { ConsoleKey.UpArrow, Keys.Up },
            { ConsoleKey.S, Keys.Down },
            { ConsoleKey.DownArrow, Keys.Down },
            { ConsoleKey.A, Keys.Left },
            { ConsoleKey.LeftArrow, Keys.Left },
            { ConsoleKey.D, Keys.Right },
            { ConsoleKey.RightArrow, Keys.Right },
            { ConsoleKey.Escape, Keys.Escape },
            { ConsoleKey.Enter, Keys.Enter },
            { ConsoleKey.H, Keys.H },
            { ConsoleKey.E, Keys.E },
            { ConsoleKey.O, Keys.O },
            { ConsoleKey.Y, Keys.Y },
            { ConsoleKey.M, Keys.M }
        };

    public event IGameInput.KeyPressEventHandler? KeyPressed;

    public void Run()
    {
        while (gameState.IsRunning)
        {
            var key = Console.ReadKey(true).Key;

            if (!_keysConversionMap.TryGetValue(key, out var value)) continue;

            KeyPressed?.Invoke(new KeyPressEventArgs(value));
        }
    }
}