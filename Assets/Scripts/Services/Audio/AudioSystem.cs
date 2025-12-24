public class AudioSystem : SaveableSystem<AudioSettings> {
    private readonly IAudioService _audioService;
    public override string SaveKey => "audio_settings";

    public AudioSystem(IAudioService audioService, ISaveLoadService saveLoadService) : base(saveLoadService) {
        _audioService = audioService;
    }

    public override AudioSettings CaptureState() {
        var settings = new AudioSettings();
        foreach (var channelType in _audioService.GetSupportedChannelsTypes()) {
            settings.channels.Add(new AudioChannel {
                ChannelType = channelType,
                Volume = _audioService.GetVolume(channelType)
            });
        }
        return settings;
    }

    public override void ApplyState(AudioSettings state) {
        foreach (var channel in state.channels) {
            _audioService.SetVolume(channel.ChannelType, channel.Volume);
        }
    }
}