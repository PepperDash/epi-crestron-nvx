using Crestron.SimplSharp;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput : INvxDeviceWithHardware
    {
        ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; }
        ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; }
        ReadOnlyDictionary<uint, StringFeedback> CurrentResolution { get; } 
    }
}