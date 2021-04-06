using NvxEpi.Abstractions.Device;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ISecondaryAudioStream : INvxDevice, ISecondaryAudioAddress
    {  
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }
    }
}