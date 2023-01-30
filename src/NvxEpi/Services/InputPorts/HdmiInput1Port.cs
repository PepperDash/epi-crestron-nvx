using System;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class HdmiInput1Port
    {
        public static void AddRoutingPort(ICurrentVideoInput device)
        {
            if (device.Hardware.HdmiIn != null && device.Hardware.HdmiIn[1] != null)
            {
                var hdmi = device.Hardware.HdmiIn[1];
                var port = new RoutingInputPortWithVideoStatuses(
                    DeviceInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    DeviceInputEnum.Hdmi1,
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
            else if (device.Hardware.DmIn != null)
            {
                var dm = device.Hardware.DmIn;
                var port = new RoutingInputPortWithVideoStatuses(
                    DeviceInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    DeviceInputEnum.Hdmi1,
                    device,
                    new VideoStatusFuncsWrapper
                    {
                        HasVideoStatusFunc = () => true,
                        HdcpStateFeedbackFunc = () => dm.HdcpCapabilityFeedback.ToString(),
                        VideoResolutionFeedbackFunc =
                            () =>
                                string.Format("{0}x{1}",
                                    dm.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                                    dm.VideoAttributes.VerticalResolutionFeedback.UShortValue),
                        VideoSyncFeedbackFunc = () => dm.SyncDetectedFeedback.BoolValue
                    });

                dm.InputStreamChange += (stream, args) => port.VideoStatus.FireAll();
                dm.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

                device.InputPorts.Add(port);

                foreach (var videoStatusOutput in port.VideoStatus.ToList().Where(x => x != null))
                    device.Feedbacks.Add(videoStatusOutput);
            }
            else
            {
                throw new NotSupportedException("hdmi 1");
            }
        }
    }
}