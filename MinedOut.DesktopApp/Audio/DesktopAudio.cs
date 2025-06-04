using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MinedOut.Core;

namespace MinedOut.DesktopApp.Audio;

public class DesktopAudio : IAudio
{
    private Song? _backgroundMusic;
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
        if (_backgroundMusic == null) return;

        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_backgroundMusic);
    }

    public void LoadContent(ContentManager content)
    {
        _deathSound = content.Load<SoundEffect>("audio/death");
        _explosionSound = content.Load<SoundEffect>("audio/explosion");
        _moveSound = content.Load<SoundEffect>("audio/move");
        _nextLevelSound = content.Load<SoundEffect>("audio/nextLevel");
        _backgroundMusic = content.Load<Song>("audio/backgroundMusic");
    }
}