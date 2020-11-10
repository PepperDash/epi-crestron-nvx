using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Output
{
    public class HdmiOutput : IHdmiOutput
    {
        private readonly INvxHardware _device;
        private readonly BoolFeedback _disabledByHdcp;
        private readonly IntFeedback _horizontalResolution;

        public HdmiOutput(INvx35XHardware device)
        {
            _device = device;

            _disabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(device.Hardware);
            _horizontalResolution = HorizontalResolutionFeedback.GetFeedback(device.Hardware);

            Feedbacks.Add(DisabledByHdcp);
            Feedbacks.Add(HorizontalResolution);
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _disabledByHdcp; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _horizontalResolution; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _device.SecondaryAudioAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }
    }
}