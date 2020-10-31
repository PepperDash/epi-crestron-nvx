using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.HdmiInput
{
    public class HdmiInput : IHdmiInput
    {
        private readonly INvxHardware _device;

        protected readonly Dictionary<uint, IntFeedback> _hdcpCapability =
            new Dictionary<uint, IntFeedback>();

        protected readonly Dictionary<uint, BoolFeedback> _syncDetected =
            new Dictionary<uint, BoolFeedback>();

        public HdmiInput(INvxHardware device)
        {
            _device = device;

            Initialize();
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

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; private set; }

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

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; private set; }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        private void Initialize()
        {
            var nvx35X = Hardware as DmNvx35x;
            if (nvx35X != null)
            {
                _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(nvx35X));
                _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(nvx35X));
                _hdcpCapability.Add(2, Hdmi2HdcpCapabilityValueFeedback.GetFeedback(nvx35X));
                _syncDetected.Add(2, Hdmi2SyncDetectedFeedback.GetFeedback(nvx35X));
            }

            var nvxe3X = Hardware as DmNvxE3x;
            if (nvxe3X != null)
            {
                _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(nvxe3X));
                _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(nvxe3X));
            }

            foreach (var feedback in _hdcpCapability.Values)
                Feedbacks.Add(feedback);

            foreach (var feedback in _syncDetected.Values)
                Feedbacks.Add(feedback);

            HdcpCapability = new ReadOnlyDictionary<uint, IntFeedback>(_hdcpCapability);
            SyncDetected = new ReadOnlyDictionary<uint, BoolFeedback>(_syncDetected);
        }
    }
}