using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class UsbcInput1Port
{
    public static void AddRoutingPort(ICurrentVideoInput device)
    {
        var nvx38xDevice = device.Hardware as DmNvx38x;
        if (nvx38xDevice == null)
        {
            Debug.LogError("Device is not an NVX38X device. Cannot add UsbcInput1Port.");
            return;
        }

        if (nvx38xDevice.UsbcIn != null && nvx38xDevice.UsbcIn[1] != null)
        {
            var usbc = nvx38xDevice.UsbcIn[1];
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
                FeedbackMatchObject = eSfpVideoSourceTypes.Usbc1,
            };

            device.IsOnline.OutputChange += (sender, args) => port.VideoStatus.FireAll();
            usbc.StreamChange += (stream, args) => port.VideoStatus.FireAll();
            usbc.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

            device.InputPorts.Add(port);

            // VideoStatus feedbacks are intentionally not added to device.Feedbacks here.

            // HdmiInput1Port already registered them under the same hardcoded keys

            // (HasVideoStatusFeedback, etc.). Adding them again throws ArgumentException and

            // aborts AddRoutingPorts(), so every port after this one (including the encoder

            // stream output) is never registered. Same fix as HdmiInput2Port.
        }
        else if (nvx38xDevice.DmIn != null)
        {
            var dm = nvx38xDevice.DmIn;
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

            // VideoStatus feedbacks are intentionally not added to device.Feedbacks here.

            // HdmiInput1Port already registered them under the same hardcoded keys

            // (HasVideoStatusFeedback, etc.). Adding them again throws ArgumentException and

            // aborts AddRoutingPorts(), so every port after this one (including the encoder

            // stream output) is never registered. Same fix as HdmiInput2Port.
        }
        else
        {
            throw new NotSupportedException("usbc 1");
        }
    }
}
