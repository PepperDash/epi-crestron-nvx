using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.RoutingPortExtensions
{
    public static class RoutingPorts
    {
        public static void AddRoutingPortToDevice(this Enumeration<VideoInputEnum> input, 
            NvxDevice parent)
        {
            if (input == VideoInputEnum.Stream)
            {
                var port = new RoutingInputPort(input.Name, eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Streaming, input, parent);

                parent.InputPorts.Add(port);
            }

            if (input == VideoInputEnum.Hdmi1)
            {
                var port =  new RoutingInputPortWithVideoStatuses(input.Name, eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi, input, parent, parent.Hardware.HdmiIn[1].FromHdmiInput());
                                
                parent.InputPorts.Add(port);
            }

            if (input == VideoInputEnum.Hdmi2)
            {
                var port =  new RoutingInputPortWithVideoStatuses(input.Name, eRoutingSignalType.AudioVideo, 
                    eRoutingPortConnectionType.Hdmi, input, parent, parent.Hardware.HdmiIn[2].FromHdmiInput());

                parent.InputPorts.Add(port);
            }
        }

        public static void AddRoutingPortToDevice(this Enumeration<VideoOutputEnum> output,
            NvxDevice parent)
        {
            if (output == VideoOutputEnum.Stream)
            {
                var port = new RoutingOutputPort(output.Name, eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Streaming, output, parent);

                parent.OutputPorts.Add(port);
            }

            if (output == VideoOutputEnum.Hdmi)
            {
                var port = new RoutingOutputPort(output.Name, eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi, output, parent);

                parent.OutputPorts.Add(port);
            }
        }

        public static void AddRoutingPortToDevice(this Enumeration<AudioInputEnum> input,
            NvxDevice parent)
        {
            if (input == AudioInputEnum.AnalogAudio)
            {
                var port = new RoutingInputPort(input.Name, eRoutingSignalType.Audio,
                    eRoutingPortConnectionType.LineAudio, input, parent);

                parent.InputPorts.Add(port);
            }

            if (input == AudioInputEnum.NaxAudio)
            {
                var port = new RoutingInputPort(input.Name, eRoutingSignalType.Audio,
                    eRoutingPortConnectionType.Streaming, input, parent);

                parent.InputPorts.Add(port);
            }
        }

        public static void AddRoutingPortToDevice(this Enumeration<AudioOutputEnum> input,
            NvxDevice parent)
        {
            if (input == AudioOutputEnum.Analog)
            {
                var port = new RoutingOutputPort(input.Name, eRoutingSignalType.Audio,
                    eRoutingPortConnectionType.LineAudio, input, parent);

                parent.OutputPorts.Add(port);
            }
        }

        private static VideoStatusFuncsWrapper FromHdmiInput(this HdmiInWithColorSpaceMode port)
        {
            return new VideoStatusFuncsWrapper()
            {
                HasVideoStatusFunc = () => true,
                HdcpActiveFeedbackFunc = () => port.HdcpSupportOnFeedback.BoolValue,
                HdcpStateFeedbackFunc = () => port.HdcpCapabilityFeedback.ToString(),
                VideoResolutionFeedbackFunc = () => "Not supported",
                VideoSyncFeedbackFunc = () => port.SyncDetectedFeedback.BoolValue
            };
        }
    }
}