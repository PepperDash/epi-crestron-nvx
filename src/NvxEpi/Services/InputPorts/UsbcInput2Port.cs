using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class UsbcInput2Port
{
    public static void AddRoutingPort(ICurrentVideoInput device)
    {
        var nvx38xDevice = device as INvx38XHardware;
        if (nvx38xDevice == null)
        {
            Debug.LogError("Device is not an NVX38X device. Cannot add UsbcInput2Port.");
            return;
        }

        if (nvx38xDevice.Hardware.UsbcIn != null && nvx38xDevice.Hardware.UsbcIn[2] != null)
        {
            var usbc = nvx38xDevice.Hardware.UsbcIn[2];
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Usbc2.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.UsbC,
                DeviceInputEnum.Usbc2,
                device,
                new VideoStatusFuncsWrapper
                {
                    HasVideoStatusFunc = () => true,
                    HdcpStateFeedbackFunc = () => usbc.HdcpCapabilityFeedback.ToString(),
                    VideoResolutionFeedbackFunc = () =>
                        string.Format(
                            "{0}x{1}",
                            usbc.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                            usbc.VideoAttributes.VerticalResolutionFeedback.UShortValue
                        ),
                    VideoSyncFeedbackFunc = () => usbc.SyncDetectedFeedback.BoolValue,
                }
            )
            {
                FeedbackMatchObject = eSfpVideoSourceTypes.Usbc2,
            };

            device.IsOnline.OutputChange += (sender, args) => port.VideoStatus.FireAll();
            usbc.StreamChange += (stream, args) => port.VideoStatus.FireAll();
            usbc.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

            device.InputPorts.Add(port);

            foreach (var videoStatusOutput in port.VideoStatus.ToList().Where(x => x != null))
                device.Feedbacks.Add(videoStatusOutput);
        }
        else if (nvx38xDevice.Hardware.DmIn != null)
        {
            var dm = nvx38xDevice.Hardware.DmIn;
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Usbc2.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.UsbC,
                DeviceInputEnum.Usbc2,
                device,
                new VideoStatusFuncsWrapper
                {
                    HasVideoStatusFunc = () => true,
                    HdcpStateFeedbackFunc = () => dm.HdcpCapabilityFeedback.ToString(),
                    VideoResolutionFeedbackFunc = () =>
                        string.Format(
                            "{0}x{1}",
                            dm.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                            dm.VideoAttributes.VerticalResolutionFeedback.UShortValue
                        ),
                    VideoSyncFeedbackFunc = () => dm.SyncDetectedFeedback.BoolValue,
                }
            );

            dm.InputStreamChange += (stream, args) => port.VideoStatus.FireAll();
            dm.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

            device.InputPorts.Add(port);

            foreach (var videoStatusOutput in port.VideoStatus.ToList().Where(x => x != null))
                device.Feedbacks.Add(videoStatusOutput);
        }
        else
        {
            throw new NotSupportedException("usbc 2");
        }
    }
}
