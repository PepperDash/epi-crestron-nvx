﻿using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Output
{
    public class HdmiOutput : IHdmiOutput
    {
        private readonly INvxDeviceWithHardware _device;

        public HdmiOutput(INvxDeviceWithHardware device)
        {
            _device = device;

            DisabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(device.Hardware);
            HorizontalResolution = HorizontalResolutionFeedback.GetFeedback(device.Hardware);
            EdidManufacturer = HdmiOutputEdidFeedback.GetFeedback(device.Hardware);
            VideoAspectRatioMode = VideoAspectRatioModeFeedback.GetFeedback(device.Hardware);

            device.Feedbacks.Add(DisabledByHdcp);
            device.Feedbacks.Add(HorizontalResolution);
            device.Feedbacks.Add(VideoAspectRatioMode);
            device.Feedbacks.Add(EdidManufacturer);
            device.Feedbacks.Add(VideoAspectRatioModeFeedbackName.GetFeedback(device.Hardware));
        }

        public BoolFeedback DisabledByHdcp { get; private set; }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public IntFeedback HorizontalResolution { get; private set; }

        public StringFeedback EdidManufacturer { get; private set; }

        public IntFeedback VideoAspectRatioMode { get; private set; }

        public string Key
        {
            get { return _device.Key; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }
    }
}