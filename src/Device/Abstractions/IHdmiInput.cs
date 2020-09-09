using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Device.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHdmiInput
    {
        HdmiInWithColorSpaceMode Input { get; }
        BoolFeedback SyncDetected { get; }
        StringFeedback CapabilityName { get; }
        IntFeedback CapabilityValue { get; }
    }

    public interface IHdmiInputs : INvxDevice
    {
        ReadOnlyDictionary<uint, IHdmiInput> HdmiInputs { get; }
    }
}