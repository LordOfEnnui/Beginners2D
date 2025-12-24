using System.Collections.Generic;

public interface IAudioService {
    void SetVolume(AudioChannelType channel, float normalizedVolume);
    float GetVolume(AudioChannelType channel);
    List<AudioChannelType> GetSupportedChannelsTypes();
}