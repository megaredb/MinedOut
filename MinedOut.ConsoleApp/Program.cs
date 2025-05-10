using MinedOut.ConsoleApp.Audio;
using MinedOut.ConsoleApp.Input;
using MinedOut.ConsoleApp.Renderer;
using MinedOut.Core;

namespace MinedOut.ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var gameCore = new GameCore<ConsoleRenderer, ConsoleGameInput, ConsoleAudio>();

        Console.CancelKeyPress += delegate { gameCore.GameState.Stop(); };

        gameCore.Run();
    }
}