using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions
{
    public interface INvxDevice : IRoutingInputsOutputs, IMulticastAddress,
        IHasFeedback, IOnline, ITransmitterReceiver, IKeyName, IDeviceId, IStreamUrl, ISecondaryAudioAddress
    {
        StringFeedback VideoName { get; }
        StringFeedback AudioName { get; }
    }
}