using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions
{
    public interface INvxDevice : IRoutingInputsOutputs,
        IHasFeedback, IOnline, ITransmitterReceiver, IKeyName, IDeviceId
    {
        StringFeedback VideoName { get; }
        StringFeedback AudioName { get; }
    }
}