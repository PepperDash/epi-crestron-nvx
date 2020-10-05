using System;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public abstract class UsbStream : IUsbStream
    {
        public static IUsbStream GetUsbStream(INvx35XHardware device, NvxUsbProperties props)
        {
            try
            {
                if (props == null)
                    return new UsbLocalStream(device, new NvxUsbProperties() { UsbId = 0, UsbMode = String.Empty });
 
                var mode = (DmNvxUsbInput.eUsbMode) Enum.Parse(typeof (DmNvxUsbInput.eUsbMode), props.UsbMode, true);

                switch (mode)
                {
                    case DmNvxUsbInput.eUsbMode.Local:
                        return new UsbLocalStream(device, props);
                    case DmNvxUsbInput.eUsbMode.Remote:
                        return new UsbRemoteStream(device, props);
                    default:
                        throw new ArgumentOutOfRangeException("usb mode");
                }
            }
            catch (ArgumentException ex)
            {
                Debug.Console(1, "Cannot set usb mode, argument not resolved:{0}", ex.Message);
                throw;
            }
        }

        private readonly INvx35XHardware _device;

        protected UsbStream(INvx35XHardware device, NvxUsbProperties props)
        {
            _device = device;
            UsbId = props.UsbId;

            device.Feedbacks.Add(UsbModeFeedback.GetFeedback(Hardware));

            Hardware.OnlineStatusChange += (currentDevice, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                Hardware.UsbInput.AutomaticUsbPairingEnabled();
            };
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

        DmNvxBaseClass INvxHardware.Hardware
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

        public DmNvx35x Hardware
        {
            get { return _device.Hardware; }
        }

        public abstract bool IsRemote { get; }

        public abstract StringFeedback UsbAddress { get; }
        public int UsbId { get; private set; }

        class UsbLocalStream : UsbStream
        {
            private readonly StringFeedback _usbAddress;

            public UsbLocalStream(INvx35XHardware device, NvxUsbProperties props)
                : base(device, props)
            {
                _usbAddress = UsbLocalAddressFeedback.GetFeedback(Hardware);
                device.Feedbacks.Add(UsbAddress);

                Hardware.OnlineStatusChange +=
                    (currentDevice, args) => Hardware.UsbInput.Mode = DmNvxUsbInput.eUsbMode.Local;
            }

            public override bool IsRemote
            {
                get { return true; }
            }

            public override sealed StringFeedback UsbAddress
            {
                get { return _usbAddress; }
            }
        }

        class UsbRemoteStream : UsbStream
        {
            private readonly StringFeedback _usbAddress;

            public UsbRemoteStream(INvx35XHardware device, NvxUsbProperties props)
                : base(device, props)
            {
                _usbAddress = UsbRemoteAddressFeedback.GetFeedback(Hardware);
                device.Feedbacks.Add(UsbAddress);

                Hardware.OnlineStatusChange +=
                    (currentDevice, args) => Hardware.UsbInput.Mode = DmNvxUsbInput.eUsbMode.Remote;
            }

            public override bool IsRemote
            {
                get { return false; }
            }

            public override sealed StringFeedback UsbAddress
            {
                get { return _usbAddress; }
            }
        }
    }
}