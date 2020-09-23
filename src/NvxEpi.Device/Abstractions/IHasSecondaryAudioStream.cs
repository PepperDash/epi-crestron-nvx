using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasSecondaryAudioStream : INvxDevice
    {
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }
        StringFeedback SecondaryAudioMulticastAddress { get; }
    }
}