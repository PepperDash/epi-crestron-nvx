using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface ISecondaryAudioStreamRouting : ISecondaryAudioStream
    {
        IntFeedback CurrentAudioRouteValue { get; }
        StringFeedback CurrentAudioRouteName { get; }
    }
}