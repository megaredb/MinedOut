using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MinedOut.Core;

namespace MinedOut.DesktopApp.Audio;

public class DesktopAudio : IAudio
{
    private SoundEffect? _deathSound;
    private SoundEffect? _explosionSound;
    private SoundEffect? _moveSound;
    private SoundEffect? _nextLevelSound;

    public void PlayMoveSound()
    {
        _moveSound?.Play();
    }

    public void PlayDieSound()
    {
        _explosionSound?.Play();
        _deathSound?.Play();
    }

    public void PlayNextLevelSound()
    {
        _nextLevelSound?.Play();
    }

    public void PlayBackgroundMusic()
    {
        // TODO
    }


    public void LoadContent(ContentManager content)
    {
        _deathSound = content.Load<SoundEffect>("audio/death");
        _explosionSound = content.Load<SoundEffect>("audio/explosion");
        _moveSound = content.Load<SoundEffect>("audio/move");
        _nextLevelSound = content.Load<SoundEffect>("audio/nextLevel");
    }
}