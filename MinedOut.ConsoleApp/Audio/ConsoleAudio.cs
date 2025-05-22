using MinedOut.Core;
using MinedOut.Core.Logic;
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
    private const string BackgroundMusicName = "backgroundMusic.mp3";

    private readonly MiniAudioEngine _audioEngine;

    private readonly GameState _gameState;

    public ConsoleAudio(GameState gameState)
    {
        _gameState = gameState;
        _gameState.GameStopped += () => { };
        _audioEngine = new MiniAudioEngine(44100, Capability.Playback);
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

    public void PlayBackgroundMusic()
    {
        PlaySound(BackgroundMusicName, true);
    }

    public void PlaySound(string soundName, bool loop = false)
    {
        new Thread(() =>
        {
            var filePath = $"{AudioDataFolder}/{soundName}";

            using (var fileStream = File.OpenRead(filePath))
            {
                var player = new SoundPlayer(new StreamDataProvider(fileStream));
                player.IsLooping = loop;

                Mixer.Master.AddComponent(player);

                player.Play();

                if (player.IsLooping)
                    while (_gameState.IsRunning)
                        Thread.Sleep(100);
                else
                    Thread.Sleep((int)Math.Ceiling(player.Duration) * 1000);

                player.Stop();

                Mixer.Master.RemoveComponent(player);
            }
        }).Start();
    }

    ~ConsoleAudio()
    {
        _audioEngine.Dispose();
    }
}