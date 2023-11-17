using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
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
        public static IUsbStreamWithHardware GetUsbStream(INvxDeviceWithHardware device, NvxUsbProperties incomingProps)
        {
            try
            {
                var props = incomingProps ?? new NvxUsbProperties()
                {
                    Mode = "local",
                    Default = string.Empty,
                    FollowVideo = false,
                    IsLayer3 = false
                };
                    //if (Debug.Level >= 0)
                    //    Debug.Console(0, device.Key, JsonConvert.SerializeObject(props, Formatting.Indented));
                
                Debug.Console(1, device, "Mode : \"{0}\", Default : \"{1}\", FollowVideo = \"{2}\"", props.Mode, props.Default, props.FollowVideo);

                return props.Mode.Equals("local", StringComparison.OrdinalIgnoreCase)
                    ? new UsbStream(device, false, props.FollowVideo, props.Default, props.IsLayer3)
                    : new UsbStream(device, true, props.FollowVideo, props.Default, props.IsLayer3);
            }
            catch (ArgumentException ex)
            {
                Debug.Console(0, "Cannot set usb mode, argument not resolved:{0}", ex.Message);
                throw;
            }
            catch (Exception e)
            {
                Debug.Console(0, "Exception in GetUsbStream : {0}", e.Message);
                throw;
            }
        }

        private readonly INvxDeviceWithHardware _device;
        private readonly StringFeedback _usbLocalId;
        private readonly ReadOnlyDictionary<uint, StringFeedback> _usbRemoteIds;
        private readonly bool _isRemote;

        private UsbStream(INvxDeviceWithHardware device, bool isRemote, bool followStream, string defaultPair, bool isLayer3)
        {
            _device = device;
            _isRemote = isRemote;
            _usbLocalId = UsbLocalAddressFeedback.GetFeedback(device.Hardware);
            _usbRemoteIds = UsbRemoteAddressFeedback.GetFeedbacks(device.Hardware);

            device.Feedbacks.AddRange(new Feedback[]
                {
                    _usbLocalId,
                    new BoolFeedback("UsbFollowsVideoStream", () => !followStream || IsTransmitter), 
                    UsbRemoteAddressFeedback.GetFeedback(device.Hardware),
                    UsbModeFeedback.GetFeedback(Hardware),
                    UsbStatusFeedback.GetFeedback(Hardware),
                    UsbRouteFeedback.GetFeedback(device.Hardware)
                });



            foreach (var item in _usbRemoteIds.Values)
                _device.Feedbacks.Add(item);

            Hardware.OnlineStatusChange += (currentDevice, args) =>
                {
                    if (!args.DeviceOnLine || Hardware.UsbInput == null)
                        return;

                    Hardware.UsbInput.AutomaticUsbPairingEnabled();
                    Hardware.UsbInput.Mode = IsRemote ? DmNvxUsbInput.eUsbMode.Remote : DmNvxUsbInput.eUsbMode.Local;
                    Hardware.UsbInput.TransportMode = isLayer3 ? DmNvxUsbInput.eUsbTransportMode.Layer3 : DmNvxUsbInput.eUsbTransportMode.Layer2;
                    if (!followStream || IsTransmitter)
                        return;

                    SetDefaultStream(isRemote, defaultPair);
                };

            if (Hardware.UsbInput == null)
            {
                return;
            }

            Hardware.UsbInput.UsbInputChange += UsbInput_UsbInputChange;

            if (!followStream || IsTransmitter)
            {
                Debug.Console(1, device, "Will not Follow Stream!");
                return;
            }

            var stream = device as ICurrentStream;
            if (stream == null)
            {
                return;
            }

            stream.StreamUrl.OutputChange += (sender, args) => FollowCurrentRoute(args.StringValue);
        }

        void UsbInput_UsbInputChange(object sender, Crestron.SimplSharpPro.DeviceSupport.GenericEventArgs args)
        {
            if (args.EventId == UsbInputEventIds.RemoteDeviceIdFeedbackEventId)
            {
                var feedback =_device.Feedbacks.FirstOrDefault(o => o.Key == UsbRouteFeedback.Key);
                feedback.FireUpdate();
            }
        }

        public void MakeUsbRoute(IUsbStreamWithHardware hardware)
        {
            if (hardware == null || hardware.Hardware.UsbInput == null)
            {
                Debug.Console(0, this, "Unable to make USB Route - hardware is null");
                return;
            }

            Debug.Console(0, this, "Trying USB Route {0}", hardware.UsbLocalId.StringValue);
            
            ClearCurrentUsbRoute();
            if (String.IsNullOrEmpty(hardware.UsbLocalId.StringValue)) return;


            /*else*/
            if (IsRemote && !hardware.IsRemote)
            {
                Debug.Console(1, this, "Routing to Local from New Route : {0}!", hardware.Name);

                hardware.AddRemoteUsbStreamToLocal(this);
            }
            else if (!IsRemote && hardware.IsRemote)
            {
                Debug.Console(1, this, "Routing to Remote from New Route : {0}!", hardware.Name);

                this.AddRemoteUsbStreamToLocal(hardware);
            }
            else
                Debug.Console(1, this, "Cannot route usb to device : {0}", hardware.Key);
        }

        private void FollowCurrentRoute(string streamUrl)
        {
            if (String.IsNullOrEmpty(streamUrl))
            {
                ClearCurrentUsbRoute();
                return;
            }

            var result = DeviceManager
                .AllDevices
                .OfType<IStreamWithHardware>()
                .FirstOrDefault(x =>
                {
                    if (x.StreamUrl == null)
                    {
                        Debug.Console(1, this, "StreamUrl Is Null!");
                        return false;
                    }
                    if (String.IsNullOrEmpty(x.StreamUrl.StringValue))
                    {
                        Debug.Console(1, this, "StreamUrl Is Empty!");
                        return false;
                    }
                    Debug.Console(1, this, "StreamUrl Is Valid!");

                    return x.IsTransmitter && x.StreamUrl.StringValue.Equals(streamUrl);
                }) as IUsbStreamWithHardware;


            var currentRoute = result;
            //if (currentRoute == null)
            ClearCurrentUsbRoute();
            /*else*/ if (IsRemote && !currentRoute.IsRemote)
            {
                Debug.Console(1, this, "Routing to Local from CurrentRoute : {0}!", currentRoute.Name);

                currentRoute.AddRemoteUsbStreamToLocal(this);
            }
            else if (!IsRemote && currentRoute.IsRemote)
            {
                Debug.Console(1, this, "Routing to Remote from CurrentRoute : {0}!", currentRoute.Name);

                this.AddRemoteUsbStreamToLocal(currentRoute);
            }
            else
                Debug.Console(1, this, "Cannot follow usb on device : {0}", currentRoute.Key);
        }

        public void ClearCurrentUsbRoute()
        {
            Debug.Console(1, this, "Setting remote id to : {0}", UsbStreamExt.ClearUsbValue);
            Hardware.UsbInput.RemoteDeviceId.StringValue = UsbStreamExt.ClearUsbValue;
            foreach (var usb in Hardware.UsbInput.RemoteDeviceIds)
            {
                usb.StringValue = UsbStreamExt.ClearUsbValue;
            }
            if(Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                Hardware.UsbInput.RemovePairing();
            ClearRemoteUsbRoute();
        }

        public void ClearRemoteUsbRoute()
        {
            foreach (var item in
                DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>()
                    .Where(device => device.Hardware.UsbInput.RemoteDeviceIdFeedback.StringValue.Equals(Hardware.UsbInput.LocalDeviceIdFeedback.StringValue))
                )
            {
                item.Hardware.UsbInput.RemoteDeviceId.StringValue = UsbStreamExt.ClearUsbValue;
                foreach (var id in item.Hardware.UsbInput.RemoteDeviceIds)
                {
                    if (!id.StringValue.Equals(UsbStreamExt.ClearUsbValue, StringComparison.OrdinalIgnoreCase))
                    {
                        id.StringValue = UsbStreamExt.ClearUsbValue;
                    }
                }
                if (item.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    item.Hardware.UsbInput.RemovePairing();
            }
        }

        private static void ClearRemoteUsbStreamToLocal(string usbId)
        {
            var results =
                DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>().Where(x => !x.IsRemote);

            IUsbStreamWithHardware local = null;
            uint index = 0;
            foreach (var usbStream in results)
            {
                foreach (var item in usbStream.UsbRemoteIds.Where(s =>
                {
                    if (s.Value == null)
                    {
                        
                        return false;
                    }

                    return !String.IsNullOrEmpty(s.Value.StringValue) && s.Value.StringValue.Equals(usbId);
                }))
                {
                    local = usbStream;
                    index = item.Key;
                }
            }

            if (local == null)
                return;
            /*
            var inputSig = local
                .Hardware
                .UsbInput
                .RemoteDeviceIds[index];

            if (inputSig == null)
            {
                Debug.Console(0, local, "Somehow input sig and index:{0} doesn't exist", index);
                return;
            }
             */

            Debug.Console(1, local, "Setting remote id to : {0}", UsbStreamExt.ClearUsbValue);
            local.Hardware.UsbInput.RemoteDeviceId.StringValue = UsbStreamExt.ClearUsbValue;
            foreach (var usb in local.Hardware.UsbInput.RemoteDeviceIds)
            {
                usb.StringValue = UsbStreamExt.ClearUsbValue;
            }
            if(local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                local.Hardware.UsbInput.RemovePairing();
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

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }
    }
}