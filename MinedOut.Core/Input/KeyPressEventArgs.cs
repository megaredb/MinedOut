namespace MinedOut.Core.Input;

public class KeyPressEventArgs(Keys key) : EventArgs
{
    public readonly Keys Key = key;
}