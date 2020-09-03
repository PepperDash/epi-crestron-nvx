using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IVideoStreamRouting : INvxDevice
    {
        IntFeedback CurrentVideoRouteValue { get; }
        StringFeedback CurrentVideoRouteName { get; }
    }
}