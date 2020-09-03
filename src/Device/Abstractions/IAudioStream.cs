using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IAudioStream : INvxDevice
    {
        BoolFeedback IsStreamingAudio { get; }
        StringFeedback AudioStreamStatus { get; }
        StringFeedback AudioMulticastAddress { get; }
    }
}