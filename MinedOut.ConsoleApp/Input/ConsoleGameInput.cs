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
            { ConsoleKey.Enter, Keys.Enter }
        };

    public event IGameInput.KeyPressEventHandler? KeyPressed;

    public static IGameInput CreateInstance(GameState gameState)
    {
        return new ConsoleGameInput(gameState);
    }

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