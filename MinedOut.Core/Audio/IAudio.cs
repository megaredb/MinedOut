namespace MinedOut.Core;

public interface IAudio
{
    public static abstract IAudio CreateInstance();

    public void PlaySound(string soundName);
    public void PlayMoveSound();
    public void PlayDieSound();
    public void PlayNextLevelSound();
}