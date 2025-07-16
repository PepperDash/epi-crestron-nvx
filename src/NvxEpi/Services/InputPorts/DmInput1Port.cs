using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class DmInput1Port
{
    public static void AddRoutingPort(ICurrentVideoInput device)
    {
        if (device.Hardware.DmIn != null)
        {
            var dm = device.Hardware.DmIn;
            var port = new RoutingInputPortWithVideoStatuses(
                DeviceInputEnum.Dm1.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.HdBaseT,
                DeviceInputEnum.Dm1,
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
            throw new NotSupportedException("dm 1");
        }
    }
}