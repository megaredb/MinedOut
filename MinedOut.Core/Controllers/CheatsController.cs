using System.Text;
using MinedOut.Core.Input;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.Core.Controllers;

public class CheatsController : Controller
{
    private const string CheatCode = "HESOYAM";
    private readonly StringBuilder _buffer = new();
    private readonly IGameInput _gameInput;
    private readonly GameState _gameState;

    public CheatsController(GameState gameState, IGameInput gameInput)
    {
        _gameState = gameState;
        _gameInput = gameInput;

        _gameInput.KeyPressed += OnInputGained;
        _gameState.ScreenChanged += () => { _buffer.Clear(); };
    }

    public void OnInputGained(KeyPressEventArgs eventArgs)
    {
        if (_gameState.Screen != Screen.Game) return;

        var keyChar = eventArgs.Key switch
        {
            Keys.H => 'H',
            Keys.E => 'E',
            Keys.Down => 'S',
            Keys.O => 'O',
            Keys.Y => 'Y',
            Keys.Left => 'A',
            Keys.M => 'M',
            _ => '\0'
        };

        if (keyChar == '\0')
        {
            _buffer.Clear();
            return;
        }

        _buffer.Append(keyChar);

        if (_buffer.Length > CheatCode.Length) _buffer.Remove(0, 1);

        if (_buffer.ToString() != CheatCode)
            return;

        var player = _gameState.World.Entities.Find(e => e is Player);

        if (player is null) return;

        Vector2I? exitPosition = null;

        for (var x = 0; x < _gameState.World.Width; x++)
        for (var y = 0; y < _gameState.World.Height; y++)
            if (_gameState.World[x, y] is Exit)
            {
                exitPosition = new Vector2I(x, y);
                break;
            }

        if (exitPosition is null) return;

        var path = AStar.FindPath(player.Position, exitPosition, _gameState.World.Grid);

        foreach (var point in path)
        {
            var cell = _gameState.World[point.X, point.Y];

            if (cell is Air || cell is Mine) _gameState.World[point.X, point.Y] = CellsRegistry.Path;
        }

        _gameState.Redraw();

        _buffer.Clear();
    }
}