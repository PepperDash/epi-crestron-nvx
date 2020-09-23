using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasVideoStreamRouting : INvxDevice
    {
        IntFeedback CurrentVideoRouteValue { get; }
        StringFeedback CurrentVideoRouteName { get; }
    }
}