using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ISecondaryAudioStream : INvxHardware
    {
        StringFeedback SecondaryAudioAddress { get; }
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }

        void SetSecondaryAudioAddress(string address);
        void ClearSecondaryAudioStream();
    }
}