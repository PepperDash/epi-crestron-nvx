using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using System.Linq;

namespace NvxEpi.Features.Hdmi.Input
{
    public class HdmiInput : HdmiInputBase
    {        
        public HdmiInput(INvxDeviceWithHardware device)
            : base(device)
        {
            foreach (var inputNumber in device.Hardware.HdmiIn.Keys)
            {
                var capability = (device.Hardware is DmNvxE760x)
                    ? DmHdcpCapabilityValueFeedback.GetFeedback(device.Hardware)
                    : HdmiHdcpCapabilityValueFeedback.GetFeedback(device.Hardware, inputNumber);

                _capability.Add(inputNumber, capability);

                var sync = (device.Hardware is DmNvxE760x)
                    ? DmSyncDetectedFeedback.GetFeedback(device.Hardware)
                    : HdmiSyncDetectedFeedback.GetFeedback(device.Hardware, inputNumber);
                _sync.Add(inputNumber, sync);

                var inputResolution = HdmiCurrentResolutionFeedback.GetFeedback(device.Hardware, inputNumber);

                _currentResolution.Add(inputNumber, inputResolution);

                var capabilityString = HdmiHdcpCapabilityFeedback.GetFeedback(device.Hardware, inputNumber);

                _capabilityString.Add(inputNumber, capabilityString);

                var audioChannels = HdmiAudioChannelsFeedback.GetFeedback(device.Hardware, inputNumber);

                _audioChannels.Add(inputNumber, audioChannels);

                var audioFormat = HdmiAudioFormatFeedback.GetFeedback(device.Hardware, inputNumber);

                _audioFormat.Add(inputNumber, audioFormat);

                var colorSpace = HdmiColorSpaceFeedback.GetFeedback(device.Hardware, inputNumber);

                _colorSpace.Add(inputNumber, colorSpace);

                var hdrType = HdmiHdrTypeFeedback.GetFeedback(device.Hardware, inputNumber);

                _hdrType.Add(inputNumber, hdrType);

                var hdcpSupport = HdmiHdcpSupportFeedback.GetFeedback(device.Hardware, inputNumber);

                _hdcpSupport.Add(inputNumber, hdcpSupport);

                Feedbacks.Add(hdcpSupport);
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
}
