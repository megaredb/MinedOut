using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MinedOut.Core.Input;
using MonoGameKeys = Microsoft.Xna.Framework.Input.Keys;
using Keys = MinedOut.Core.Input.Keys;

namespace MinedOut.DesktopApp.Input;

public class DesktopGameInput : IGameInput
{
    private readonly Dictionary<MonoGameKeys, Keys> _keysConversionMap =
        new()
        {
            { MonoGameKeys.W, Keys.Up },
            { MonoGameKeys.Up, Keys.Up },
            { MonoGameKeys.S, Keys.Down },
            { MonoGameKeys.Down, Keys.Down },
            { MonoGameKeys.A, Keys.Left },
            { MonoGameKeys.Left, Keys.Left },
            { MonoGameKeys.D, Keys.Right },
            { MonoGameKeys.Right, Keys.Right },
            { MonoGameKeys.Escape, Keys.Escape },
            { MonoGameKeys.Enter, Keys.Enter }
        };

    private KeyboardState _oldKeyboardState;

    public event IGameInput.KeyPressEventHandler? KeyPressed;

    public void Initialize()
    {
        _oldKeyboardState = Keyboard.GetState();
    }

    public void Update()
    {
        var keyboard = Keyboard.GetState();
        var pressedKeys = keyboard.GetPressedKeys();

        foreach (var key in pressedKeys)
            if (!_oldKeyboardState.IsKeyDown(key) && _keysConversionMap.TryGetValue(key, out var value))
            {
                KeyPressed?.Invoke(new KeyPressEventArgs(value));
                break;
            }

        _oldKeyboardState = keyboard;
    }
}