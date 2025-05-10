using MinedOut.Core;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace MinedOut.ConsoleApp.Audio;

public class ConsoleAudio : IAudio
{
    private const string AudioDataFolder = "./Data/Audio";
    private const string MoveSoundName = "move.wav";
    private const string ExplosionSoundName = "explosion.wav";
    private const string DeathSoundName = "death.wav";
    private const string NextLevelSoundName = "nextLevel.wav";
    private readonly MiniAudioEngine _audioEngine;

    public ConsoleAudio()
    {
        _audioEngine = new MiniAudioEngine(44100, Capability.Playback);
    }

    public static IAudio CreateInstance()
    {
        return new ConsoleAudio();
    }

    public void PlaySound(string soundName)
    {
        new Thread(() =>
        {
            var filePath = $"{AudioDataFolder}/{soundName}";

            using (var fileStream = File.OpenRead(filePath))
            {
                var player = new SoundPlayer(new StreamDataProvider(fileStream));
                Mixer.Master.AddComponent(player);

                player.Play();
                Thread.Sleep((int)Math.Ceiling(player.Duration) * 1000);
                player.Stop();

                Mixer.Master.RemoveComponent(player);
            }
        }).Start();
    }

    public void PlayMoveSound()
    {
        PlaySound(MoveSoundName);
    }

    public void PlayDieSound()
    {
        PlaySound(ExplosionSoundName);
        PlaySound(DeathSoundName);
    }

    public void PlayNextLevelSound()
    {
        PlaySound(NextLevelSoundName);
    }

    ~ConsoleAudio()
    {
        _audioEngine.Dispose();
    }
}