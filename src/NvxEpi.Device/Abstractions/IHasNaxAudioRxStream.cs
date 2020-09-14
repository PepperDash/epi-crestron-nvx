using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasNaxAudioRxStream
    {
        BoolFeedback IsStreamingNaxRxAudio { get; }
        StringFeedback NaxAudioRxStreamStatus { get; }
        StringFeedback NaxAudioRxMulticastAddress { get; }
    }
}