using NvxEpi.Abstractions.Device;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions
{
    public interface INvxDevice : IRoutingInputsOutputs,
        IHasFeedback, IOnline, ITransmitterReceiver, IKeyName, IDeviceId
    {

    }
}