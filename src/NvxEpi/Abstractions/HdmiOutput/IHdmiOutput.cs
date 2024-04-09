using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IHdmiOutput : INvxDeviceWithHardware
    {
        BoolFeedback DisabledByHdcp { get; }
        IntFeedback HorizontalResolution { get; }
        StringFeedback EdidManufacturer { get; }

        StringFeedback OutputResolution { get; }
    }
}