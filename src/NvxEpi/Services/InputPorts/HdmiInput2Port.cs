using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

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
            })
        {
            FeedbackMatchObject = eSfpVideoSourceTypes.Hdmi2
        };

        hdmi.StreamChange += (stream, args) => port.VideoStatus.FireAll();
        hdmi.VideoAttributes.AttributeChange += (sender, args) => port.VideoStatus.FireAll();

        device.InputPorts.Add(port);
        // VideoStatus feedbacks are intentionally not added to device.Feedbacks here.
        // HdmiInput1Port already registered them under the same hardcoded keys
        // (HasVideoStatusFeedback, etc.) — adding them again throws ArgumentException
        // and aborts AddRoutingPorts(), which prevents the stream output port from being
        // registered and causes NullReferenceException in TieLineConnector.
    }
}