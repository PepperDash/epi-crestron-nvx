using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ISecondaryAudioStream : INvx35XHardware, IOnline
    {
        StringFeedback SecondaryAudioAddress { get; }
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }
    }
}