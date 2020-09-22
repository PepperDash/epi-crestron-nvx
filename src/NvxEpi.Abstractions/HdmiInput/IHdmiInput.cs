using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput : INvxHardware
    {
        ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; }
        ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; } 
    }

    public static class HdmiInputExtensions
    {
        public static void SetHdmi1HdcpCapability(this IHdmiInput device, ushort capability)
        {
            if (device.Hardware.HdmiIn[1] == null)
                throw new NotSupportedException("hdmi1");

            var capabilityToSet = (eHdcpCapabilityType) capability;

            switch (capabilityToSet)
            {
                case eHdcpCapabilityType.HdcpSupportOff:
                    device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.HdcpAutoSupport:
                    device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.Hdcp1xSupport:
                    device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.Hdcp2_2Support:
                    device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void SetHdmi2HdcpCapability(this IHdmiInput device, ushort capability)
        {
            if (device.Hardware.HdmiIn[2] == null)
                throw new NotSupportedException("hdmi2");

            var capabilityToSet = (eHdcpCapabilityType)capability;

            switch (capabilityToSet)
            {
                case eHdcpCapabilityType.HdcpSupportOff:
                    device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.HdcpAutoSupport:
                    device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.Hdcp1xSupport:
                    device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                    break;
                case eHdcpCapabilityType.Hdcp2_2Support:
                    device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}