using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasSecondaryAudioStreamRouting : IHasSecondaryAudioStream
    {
        IntFeedback CurrentSecondaryAudioRouteValue { get; }
        StringFeedback CurrentSecondaryAudioRouteName { get; }
    }
}