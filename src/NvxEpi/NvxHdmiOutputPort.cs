using PepperDash.Essentials.Core;

namespace NvxEpi
{
    internal class NvxHdmiOutputPort(
        string key,
        INvxDevice device,
        BoolFeedback disabledByHdcpFeedback,
        StringFeedback outputResolutionFeedback,
        StringFeedback edidManufacturerFeedback) : RoutingOutputPort(key, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Hdmi, NvxDevice.NvxOutputSelector.Hdmi, device)
    {
        public BoolFeedback DisabledByHdcpFeedback { get; } = disabledByHdcpFeedback;
        public StringFeedback OutputResolutionFeedback { get; } = outputResolutionFeedback;
        public StringFeedback EdidManufacturerFeedback { get; } = edidManufacturerFeedback;
    }
}
