public class AudioManager : GameComponent, IAudioManager
{
    /// <summary>
    /// Initializes this Audio manager.
    /// </summary>
    public override void Initialize()
    {
        _previousMusicVolume = MediaPlayer.Volume = 1.0f;
        _previousSoundEffectVolume = SoundEffect.MasterVolume = 1.0f;

        // Add this instance to the game's services container.
        Game.Services.AddService<IAudioManager>(this);
    }

    // Rest of class implementation remains the same.
}
