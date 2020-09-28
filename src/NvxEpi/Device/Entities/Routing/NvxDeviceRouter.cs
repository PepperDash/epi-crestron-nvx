using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Device.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class NvxDeviceRouter : INvxDevice, IRouting
    {
        private readonly INvxDevice _device;

        public NvxDeviceRouter(INvxDevice device)
        {
            _device = device;
        }

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

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("input selector");

                switcher.HandleSwitch(inputSelector, signalType);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error executing route! : {0}", ex.Message);
            }
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

        public void UpdateDeviceId(uint id)
        {
            _device.UpdateDeviceId(id);
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }
    }
}