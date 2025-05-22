namespace MinedOut.Core.Input;

public interface IGameInput
{
    public delegate void KeyPressEventHandler(KeyPressEventArgs eventArgs);

    public event KeyPressEventHandler? KeyPressed;

    public static int GetAxis(Keys key, Keys negativeKey, Keys positiveKey)
    {
        return key switch
        {
            _ when key == negativeKey => -1,
            _ when key == positiveKey => 1,
            _ => 0
        };
    }
}