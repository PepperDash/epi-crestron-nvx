using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasHdmiOutput
    {
        IntFeedback HorizontalResolution { get; }
        BoolFeedback DisabledByHdcp { get; }
    }
}