using System;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Usb
{
    public abstract class UsbStream : IUsbStream
    {
        public static IUsbStream GetUsbStream(INvx35XHardware device, NvxUsbProperties props)
        {
            try
            {
                if (props == null)
                    return new UsbLocalStream(device, new NvxUsbProperties() { UsbId = 0, Mode = String.Empty });
 
                var mode = (DmNvxUsbInput.eUsbMode) Enum.Parse(typeof (DmNvxUsbInput.eUsbMode), props.Mode, true);

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

            UsbLocalId = UsbLocalAddressFeedback.GetFeedback(Hardware);
            UsbRemoteId = UsbRemoteAddressFeedback.GetFeedback(Hardware);

            device.Feedbacks.AddRange(new Feedback[]
            {
                UsbModeFeedback.GetFeedback(Hardware),
                UsbLocalId,
                UsbRemoteId
            });

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
        public StringFeedback UsbLocalId { get; private set; }
        public StringFeedback UsbRemoteId { get; private set; }
        public int UsbId { get; private set; }

        class UsbLocalStream : UsbStream
        {
            public UsbLocalStream(INvx35XHardware device, NvxUsbProperties props)
                : base(device, props)
            {
                Hardware.OnlineStatusChange +=
                    (currentDevice, args) =>
                    {
                        if (!args.DeviceOnLine)
                            return;

                        Hardware.UsbInput.Mode = DmNvxUsbInput.eUsbMode.Local;

                        if (!String.IsNullOrEmpty(props.Default))
                            PairUsb(props.Default);
                    }; 
            }

            public override bool IsRemote
            {
                get { return false; }
            }

            private void PairUsb(IUsbStream remote)
            {
                if (!remote.IsRemote)
                    throw new ArgumentException("remote");

                Debug.Console(1, this, "Pairing usb to : {0}", remote.Key);
                remote.UsbRemoteId.FireUpdate();
                UsbLocalId.FireUpdate();
                Hardware.UsbInput.RemoteDeviceId.StringValue = remote.UsbLocalId.StringValue;
                remote.Hardware.UsbInput.RemoteDeviceId.StringValue = UsbLocalId.StringValue;
            }

            private void PairUsb(string remote)
            {
                var remoteDevice = DeviceManager.GetDeviceForKey(remote) as IUsbStream;

                if (remoteDevice == null || !remoteDevice.IsRemote)
                    return;

                PairUsb(remoteDevice);
            }
        }

        class UsbRemoteStream : UsbStream
        {
            public UsbRemoteStream(INvx35XHardware device, NvxUsbProperties props)
                : base(device, props)
            {
                Hardware.OnlineStatusChange +=
                    (currentDevice, args) =>
                    {
                        if (!args.DeviceOnLine)
                            return;

                        Hardware.UsbInput.Mode = DmNvxUsbInput.eUsbMode.Remote;
                    };
            }

            public override bool IsRemote
            {
                get { return true; }
            }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _device.SecondaryAudioAddress; }
        }
    }
}