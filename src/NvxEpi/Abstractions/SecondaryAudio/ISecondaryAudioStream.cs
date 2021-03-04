using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ISecondaryAudioStream : INvxHardware
    {  
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }
    }
}