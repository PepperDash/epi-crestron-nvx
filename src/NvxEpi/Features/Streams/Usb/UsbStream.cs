using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Extensions;
using NvxEpi.Features.Config;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Usb
{
    public class UsbStream : IUsbStreamWithHardware
    {
        public static IUsbStreamWithHardware GetUsbStream(INvxDeviceWithHardware device, NvxUsbProperties props)
        {
            try
            {
                if (props == null || string.IsNullOrEmpty(props.Mode))
                    return new UsbStream(device, true, false, string.Empty);

                return props.Mode.Equals("local", StringComparison.OrdinalIgnoreCase) 
                    ? new UsbStream(device, false, props.FollowVideo, props.Default) 
                    : new UsbStream(device, true, props.FollowVideo, props.Default);
            }
            catch (ArgumentException ex)
            {
                Debug.Console(1, "Cannot set usb mode, argument not resolved:{0}", ex.Message);
                throw;
            }
        }

        private readonly INvxDeviceWithHardware _device;
        private readonly StringFeedback _usbLocalId;
        private readonly ReadOnlyDictionary<uint, StringFeedback> _usbRemoteIds;
        private readonly bool _isRemote;

        private UsbStream(INvxDeviceWithHardware device, bool isRemote, bool followStream, string defaultPair)
        {
            _device = device;
            _isRemote = isRemote;
            _usbLocalId = UsbLocalAddressFeedback.GetFeedback(device.Hardware);
            _usbRemoteIds = UsbRemoteAddressFeedback.GetFeedbacks(device.Hardware);

            _device.Feedbacks.AddRange(new Feedback[]
                {
                    _usbLocalId,
                    UsbRemoteAddressFeedback.GetFeedback(device.Hardware),
                    UsbModeFeedback.GetFeedback(Hardware),
                    UsbStatusFeedback.GetFeedback(Hardware)
                });

            foreach (var item in _usbRemoteIds.Values)
                _device.Feedbacks.Add(item);

            Hardware.OnlineStatusChange += (currentDevice, args) =>
                {
                    if (!args.DeviceOnLine)
                        return;

                    Hardware.UsbInput.AutomaticUsbPairingEnabled();
                    Hardware.UsbInput.Mode = IsRemote ? DmNvxUsbInput.eUsbMode.Remote : DmNvxUsbInput.eUsbMode.Local;
                    SetDefaultStream(isRemote, defaultPair);
                };

            if (!followStream || !IsTransmitter) 
                return;

            var stream = device as ICurrentStream;
            if (stream == null)
                return;

            stream.StreamUrl.OutputChange += (sender, args) => FollowCurrentRoute(args.StringValue);
        }

        private void FollowCurrentRoute(string streamUrl)
        {
            if (String.IsNullOrEmpty(streamUrl))
            {
                ClearCurrentRoute();
                return;
            }

            var result = DeviceManager
                .AllDevices
                .OfType<IStreamWithHardware>()
                .FirstOrDefault(x => x.IsTransmitter && x.StreamUrl.StringValue.Equals(streamUrl));

            var currentRoute = result as IUsbStreamWithHardware;
            if (currentRoute == null)
                ClearCurrentRoute();
            else if (IsRemote && !currentRoute.IsRemote)
                currentRoute.AddRemoteUsbStreamToLocal(this);
            else if (!IsRemote && currentRoute.IsRemote)
                this.AddRemoteUsbStreamToLocal(currentRoute);
            else
                Debug.Console(1, this, "Cannot follow usb on device : {0}", currentRoute.Key);
        }

        private void ClearCurrentRoute()
        {
            Debug.Console(1, this, "Setting remote id to : {0}", UsbStreamExt.ClearUsbValue);
            Hardware.UsbInput.RemoteDeviceId.StringValue = UsbStreamExt.ClearUsbValue;
            ClearRemoteUsbStreamToLocal(UsbLocalId.StringValue);
        }

        private static void ClearRemoteUsbStreamToLocal(string usbId)
        {
            var results =
                DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>().Where(x => !x.IsRemote);

            IUsbStreamWithHardware local = null;
            uint index = 0;
            foreach (var usbStream in results)
            {
                foreach (var item in usbStream.UsbRemoteIds.Where(s => s.Value.StringValue.Equals(usbId)))
                {
                    local = usbStream;
                    index = item.Key;
                }
            }

            if (local == null)
                return;

            var inputSig = local
                .Hardware
                .UsbInput
                .RemoteDeviceIds[index];

            if (inputSig == null)
            {
                Debug.Console(0, local, "Somehow input sig and index:{0} doesn't exist", index);
                return;
            }

            Debug.Console(1, local, "Setting remote id to : {0}", UsbStreamExt.ClearUsbValue);
            inputSig.StringValue = UsbStreamExt.ClearUsbValue;
        }

        private void SetDefaultStream(bool isRemote, string defaultPair)
        {
            if (!isRemote || String.IsNullOrEmpty(defaultPair))
                return;

            var local = DeviceManager.GetDeviceForKey(defaultPair) as IUsbStreamWithHardware;
            if (local == null)
                return;

            local.AddRemoteUsbStreamToLocal(this);
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

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback BuildFeedbacks
        {
            get { return _device.BuildFeedbacks; }
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

        public bool IsRemote
        {
            get { return _isRemote; }
        }

        public StringFeedback UsbLocalId
        {
            get { return _usbLocalId; }
        }

        public ReadOnlyDictionary<uint, StringFeedback> UsbRemoteIds
        {
            get { return _usbRemoteIds; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }
    }
}