using System.Collections.Generic;
using Crestron.SimplSharp;
using NvxEpi.Device.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHdmiInput
    {
        BoolFeedback SyncDetected { get; }
        StringFeedback CapabilityName { get; }
        IntFeedback CapabilityValue { get; }

        void SetHdcpCapability(HdcpCapabilityEnum capability);
    }

    public interface IHdmiInputs : INvxDevice
    {
        ReadOnlyDictionary<uint, IHdmiInput> HdmiInputs { get; }
    }
}