using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching;

public interface ICurrentVideoInputWithUsbc : ICurrentVideoInput, INvx38XDeviceWithHardware
{

}