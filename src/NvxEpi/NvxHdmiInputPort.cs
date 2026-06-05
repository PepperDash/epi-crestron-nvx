using PepperDash.Essentials.Core;

namespace NvxEpi
{
    internal class NvxHdmiInputPort(
        string key,
        INvxDevice device,
        NvxDevice.NvxInputSelector selector,
        StringFeedback hdcpCapability,
        IntFeedback audioChannelCount,
        StringFeedback audioFormat,
        StringFeedback colorspaceMode,
        StringFeedback hdrType,
        VideoStatusFuncsWrapper funcs) : RoutingInputPortWithVideoStatuses(key, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Hdmi, selector, device, funcs)
    {
        public StringFeedback HdcpCapability { get; } = hdcpCapability;
        public IntFeedback AudioChannelCount { get; } = audioChannelCount;
        public StringFeedback AudioFormat { get; } = audioFormat;
        public StringFeedback ColorspaceMode { get; } = colorspaceMode;
        public StringFeedback HdrType { get; } = hdrType;
    }
}
