using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IVideowallMode : IHdmiOutput, INvx35XDeviceWithHardware
    {
        IntFeedback VideowallMode { get; }
    }
}