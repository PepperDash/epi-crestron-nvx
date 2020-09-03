using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHdmiOutput : INvxDevice
    {
        IntFeedback HorizontalResolution { get; }
        BoolFeedback DisabledByHdcp { get; }
    }
}