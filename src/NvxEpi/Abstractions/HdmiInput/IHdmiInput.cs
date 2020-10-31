using System;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput : INvxHardware
    {
        ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; }
        ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; } 
    }
}