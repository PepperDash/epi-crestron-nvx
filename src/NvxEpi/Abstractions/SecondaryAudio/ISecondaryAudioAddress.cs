using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Device
{
    public interface ISecondaryAudioAddress
    {
        StringFeedback SecondaryAudioAddress { get; }
        StringFeedback TxAudioAddress { get; }
        StringFeedback RxAudioAddress { get; }
    }
}