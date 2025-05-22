using MinedOut.ConsoleApp.Audio;
using MinedOut.ConsoleApp.Input;
using MinedOut.ConsoleApp.Renderer;
using MinedOut.Core;
using MinedOut.Core.Logic;

namespace MinedOut.ConsoleApp;

internal class Program
{
    private static void Main()
    {
        if (Console.WindowWidth < 70 || Console.WindowHeight < 30)
        {
            Console.WriteLine("Window must be at least 70x30! Your window is " + Console.WindowWidth + "x" +
                              Console.WindowHeight);

            return;
        }

        var gameState = new GameState();
        var audio = new ConsoleAudio(gameState);
        var renderer = new ConsoleRenderer(gameState);
        var gameInput = new ConsoleGameInput(gameState);

        var gameCore = new GameCore(gameState, audio, gameInput);

        Console.CancelKeyPress += delegate { gameCore.GameState.Stop(); };

        renderer.Render();
        gameInput.Run();
    }
}