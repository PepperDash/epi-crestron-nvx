using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using PepperDash.Core;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxHardware : ITransmitterReceiver, IKeyName, IDeviceId
    {
        DmNvxBaseClass Hardware { get; }
    }
}