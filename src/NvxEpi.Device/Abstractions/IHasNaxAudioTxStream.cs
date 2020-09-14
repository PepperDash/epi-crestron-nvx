using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasNaxAudioTxStream : INvxDevice
    {
        BoolFeedback IsStreamingNaxTxAudio { get; }
        StringFeedback NaxAudioTxStreamStatus { get; }
        StringFeedback NaxAudioTxMulticastAddress { get; }
    }
}