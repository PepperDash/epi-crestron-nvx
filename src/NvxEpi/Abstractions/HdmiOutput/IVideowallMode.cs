using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IVideowallMode : IHdmiOutput, INvx35XHardware
    {
        IntFeedback VideowallMode { get; }
    }
}