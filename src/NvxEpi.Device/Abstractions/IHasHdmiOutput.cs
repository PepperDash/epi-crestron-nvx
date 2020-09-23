using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasHdmiOutput : INvxDevice
    {
        IntFeedback HorizontalResolution { get; }
        BoolFeedback DisabledByHdcp { get; }
    }
}