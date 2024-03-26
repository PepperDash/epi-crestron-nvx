using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Input
{
    public class HdmiInput2 : HdmiInput1
    {
        private const uint InputNumber = 2;

        public HdmiInput2(INvxDeviceWithHardware device)
            : base(device)
        {

            var capability = (device.Hardware is DmNvxE760x)
                ? DmHdcpCapabilityValueFeedback.GetFeedback(device.Hardware)
                : HdmiHdcpCapabilityValueFeedback.GetFeedback(device.Hardware, InputNumber);

            _capability.Add(InputNumber, capability);

            var sync = (device.Hardware is DmNvxE760x)
                ? DmSyncDetectedFeedback.GetFeedback(device.Hardware)
                : HdmiSyncDetectedFeedback.GetFeedback(device.Hardware, InputNumber);

            _sync.Add(InputNumber, sync);

            var inputResolution = HdmiCurrentResolutionFeedback.GetFeedback(device.Hardware, InputNumber);

            _currentResolution.Add(InputNumber, inputResolution);

            var capabilityString = HdmiHdcpCapabilityFeedback.GetFeedback(device.Hardware, InputNumber);

            var audioChannels = HdmiAudioChannelsFeedback.GetFeedback(device.Hardware, InputNumber);

            _audioChannels.Add(InputNumber, audioChannels);

            var audioFormat = HdmiAudioFormatFeedback.GetFeedback(device.Hardware, InputNumber);

            _audioFormat.Add(InputNumber, audioFormat);

            var colorSpace = HdmiColorSpaceFeedback.GetFeedback(device.Hardware, InputNumber);

            _colorSpace.Add(InputNumber, colorSpace);

            var hdrType = HdmiHdrTypeFeedback.GetFeedback(device.Hardware, InputNumber);

            _hdrType.Add(InputNumber, hdrType);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(inputResolution);
            Feedbacks.Add(capabilityString);
            Feedbacks.Add(audioChannels);
            Feedbacks.Add(audioFormat);
            Feedbacks.Add(colorSpace);
            Feedbacks.Add(hdrType);
        }
    }
}