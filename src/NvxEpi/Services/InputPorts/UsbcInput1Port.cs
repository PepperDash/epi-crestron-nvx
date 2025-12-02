using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class UsbcInput1Port
{
    public static void AddRoutingPort(ICurrentVideoInputWithUsbc device)
    {
        if (device.Hardware.UsbcIn != null && device.Hardware.UsbcIn[1] != null)
        {
            var usbc = device.Hardware.UsbcIn[1];
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Usbc1.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.UsbC,
                DeviceInputEnum.Usbc1,
                device,
                new VideoStatusFuncsWrapper
                {
                    HasVideoStatusFunc = () => true,
                    HdcpStateFeedbackFunc = () => usbc.HdcpCapabilityFeedback.ToString(),
                    VideoResolutionFeedbackFunc =
                        () =>
                            string.Format("{0}x{1}",
                                usbc.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                                usbc.VideoAttributes.VerticalResolutionFeedback.UShortValue),
                    VideoSyncFeedbackFunc = () => usbc.SyncDetectedFeedback.BoolValue
                })
            {
                FeedbackMatchObject = eSfpVideoSourceTypes.Usbc1
            }
                ;

            usbc.StreamChange += (stream, args) => port.VideoStatus.FireAll();
            usbc.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

            device.InputPorts.Add(port);

            foreach (var videoStatusOutput in port.VideoStatus.ToList().Where(x => x != null))
                device.Feedbacks.Add(videoStatusOutput);
        }
        else if (device.Hardware.DmIn != null)
        {
            var dm = device.Hardware.DmIn;
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Usbc1.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.UsbC,
                DeviceInputEnum.Usbc1,
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
            throw new NotSupportedException("usbc 1");
        }
    }
}