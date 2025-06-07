using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinedOut.Core.Logic;
using MinedOut.Core.Logic.Entities;
using MinedOut.Core.Logic.World.Cells;
using MinedOut.Core.Utilities;

namespace MinedOut.DesktopApp.Renderer;

public class DesktopRenderer
{
    private const int CellSize = 32;
    private readonly GameState _gameState;
    private readonly SpriteBatch _spriteBatch;
    private readonly Dictionary<Type, Texture2D> _textures = new();
    private SpriteFont _font = null!;
    private Vector2 _offset;

    public DesktopRenderer(GameState gameState, GraphicsDevice graphicsDevice)
    {
        _gameState = gameState;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    private int CountNearbyMines(Player player)
    {
        var count = 0;
        for (var dx = -1; dx <= 1; dx++)
        for (var dy = -1; dy <= 1; dy++)
        {
            if (((dy + 1) * 3 + dx + 1) % 2 == 0) continue;
            var position = player.Position + new Vector2I(dx, dy);
            if (position.X < 0 || position.X >= _gameState.World.Width ||
                position.Y < 0 || position.Y >= _gameState.World.Height)
                continue;

            if (_gameState.World[position.X, position.Y] is Mine)
                count++;
        }

        return count;
    }

    private Color GetMineCountColor(int count)
    {
        return count switch
        {
            0 => Color.Green,
            1 => Color.Blue,
            2 => Color.Yellow,
            3 => Color.Orange,
            _ => Color.Red
        };
    }

    public void LoadContent(Game game)
    {
        _textures[typeof(Wall)] = game.Content.Load<Texture2D>("images/wall");
        _textures[typeof(Air)] = game.Content.Load<Texture2D>("images/air");
        _textures[typeof(Mine)] = game.Content.Load<Texture2D>("images/air");
        _textures[typeof(Path)] = game.Content.Load<Texture2D>("images/path");
        _textures[typeof(Player)] = game.Content.Load<Texture2D>("images/player");
        _textures[typeof(Exit)] = game.Content.Load<Texture2D>("images/exit");
        _textures[typeof(LiveMine)] = game.Content.Load<Texture2D>("images/mine");
        _textures[typeof(BonusCoin)] = game.Content.Load<Texture2D>("images/coin");
        _textures[typeof(Robot)] = game.Content.Load<Texture2D>("images/player");

        _font = game.Content.Load<SpriteFont>("Fonts/GameFont");

        _offset = new Vector2(
            (game.GraphicsDevice.Viewport.Width - _gameState.World.Width * CellSize) / 2f,
            (game.GraphicsDevice.Viewport.Height - _gameState.World.Height * CellSize) / 2f
        );
    }

    public void Render()
    {
        if (!_gameState.IsRunning)
            return;

        _spriteBatch.Begin();

        if (_gameState.Screen == Screen.Game && !_gameState.ExitConfirmation)
        {
            for (var y = 0; y < _gameState.World.Height; y++)
            for (var x = 0; x < _gameState.World.Width; x++)
            {
                var cell = _gameState.World[x, y];
                var position = new Vector2(x * CellSize, y * CellSize) + _offset;

                _spriteBatch.Draw(_textures[cell.GetType()], position, Color.White);

                foreach (var entity in _gameState.World.Entities)
                    if (entity.Position.X == x && entity.Position.Y == y)
                    {
                        var entityType = entity.GetType();
                        var color = Color.White;
                        if (entityType == typeof(Robot)) color = Color.Red;
                        var texture = _textures[entityType];

                        _spriteBatch.Draw(texture, position, color);

                        if (entity is Player player)
                        {
                            var nearbyMines = CountNearbyMines(player);
                            var minesText = nearbyMines.ToString();
                            var textSize = _font.MeasureString(minesText);
                            var textPosition = position + new Vector2(
                                (CellSize - textSize.X) / 2,
                                (CellSize - textSize.Y) / 2
                            );
                            var minesColor = GetMineCountColor(nearbyMines);
                            _spriteBatch.DrawString(_font, minesText, textPosition, minesColor);
                        }
                    }
            }

            var scoreText = $"Score: {_gameState.Score}";
            _spriteBatch.DrawString(_font, scoreText, new Vector2(10, 10), Color.Yellow);
        }
        else
        {
            string[] lines;
            if (_gameState.ExitConfirmation)
                lines = new[]
                {
                    "Are you sure you want to exit?",
                    "",
                    "Press Enter to confirm",
                    "Press Esc to cancel"
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
                    $"Your score was: {_gameState.Score}"
                };

            var totalHeight = 0f;
            foreach (var line in lines)
                totalHeight += _font.MeasureString(line).Y;

            var yPos = (_spriteBatch.GraphicsDevice.Viewport.Height - totalHeight) / 2;

            foreach (var line in lines)
            {
                var size = _font.MeasureString(line);
                _spriteBatch.DrawString(_font, line,
                    new Vector2((_spriteBatch.GraphicsDevice.Viewport.Width - size.X) / 2, yPos),
                    Color.White);
                yPos += size.Y;
            }
        }

        _spriteBatch.End();
    }
}