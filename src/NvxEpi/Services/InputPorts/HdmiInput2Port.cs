using System;
using System.Linq;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class HdmiInput2Port
    {
        public static void AddRoutingPort(ICurrentVideoInput device)
        {
            if (device.Hardware.HdmiIn == null || device.Hardware.HdmiIn[2] == null)
                throw new NotSupportedException("hdmi 2");

            var hdmi = device.Hardware.HdmiIn[2];
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Hdmi2.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                DeviceInputEnum.Hdmi2,
                device,
                new VideoStatusFuncsWrapper
                    {
                        HasVideoStatusFunc = () => true,
                        HdcpStateFeedbackFunc = () => hdmi.HdcpCapabilityFeedback.ToString(),
                        VideoResolutionFeedbackFunc =
                            () =>
                                string.Format("{0}x{1}",
                                    hdmi.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                                    hdmi.VideoAttributes.VerticalResolutionFeedback.UShortValue),
                        VideoSyncFeedbackFunc = () => hdmi.SyncDetectedFeedback.BoolValue
                    });

            hdmi.StreamChange += (stream, args) => port.VideoStatus.FireAll();
            hdmi.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

            device.InputPorts.Add(port);
            foreach (var videoStatusOutput in port.VideoStatus.ToList().Where(x => x != null))
                device.Feedbacks.Add(videoStatusOutput);
        }
    }
}