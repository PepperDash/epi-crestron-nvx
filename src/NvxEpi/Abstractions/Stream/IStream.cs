using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxHardware
    {
        StringFeedback StreamUrl { get; }
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }

        void SetStreamUrl(string url);
        void ClearStream();
    }
}