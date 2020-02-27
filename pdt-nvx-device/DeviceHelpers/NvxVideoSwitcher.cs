using System;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Interfaces;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.DeviceHelpers
{
    public class NvxVideoSwitcher : NvxDeviceHelperBase, ISwitcher
    {
        private int _selectedInput;

        private string _key;
        public override string Key
        {
            get { return string.Format("{0} {1}", _key, this.GetType().GetCType().Name); }
        }

        public Feedback Feedback { get; set; }

        public event EventHandler RouteUpdated;

        public NvxVideoSwitcher(DeviceConfig config, DmNvxBaseClass device)
            : base(device)
        {
            _key = config.Key;
            Feedback = FeedbackFactory.GetFeedback(() => Source);

            _device.BaseEvent += (sender, args) =>
            {
                switch (args.EventId)
                {
                    case DMInputEventIds.StreamUriFeedbackEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.VideoSourceEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.StatusEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.StatusTextEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.StartEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.StopEventId:
                        OnRouteUpdated();
                        break;
                    case DMInputEventIds.MulticastAddressEventId:
                        OnRouteUpdated();
                        break;
                    default:
                        break;
                }      
            };

            _device.OnlineStatusChange += (sender, args) =>
            {
                if (args.DeviceOnLine)
                {
                    Source = _selectedInput;
                }
            };
        }

        public string CurrentlyRouted
        {
            get
            {
                var result = "No Source";

                if (_isTransmitter) return _device.Control.VideoSourceFeedback.ToString();
                if (!_device.Control.StartFeedback.BoolValue)
                {
                    return result;
                }

                var device = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.StreamUrl == _device.Control.ServerUrlFeedback.StringValue);
               
                if (device != null)
                {
                    result = device.DeviceName;
                }       

                return result;
            }
        }

        public int Source
        {
            get
            {
                var result = 0;

                if (_isTransmitter) return result;
                if (!_device.Control.StartFeedback.BoolValue)
                {
                    return result;
                }

                var device = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.StreamUrl == _device.Control.ServerUrlFeedback.StringValue);
               
                if (device != null)
                {
                    result = device.VirtualDevice;
                }       

                Debug.Console(2, this, "Video input is Virtual ID: {0}", result);
                return result;
            }
            set
            {
                if (_selectedInput != value) _selectedInput = value;
                if (_isTransmitter || !_device.IsOnline) return;

                if (value == 0) 
                {
                    Debug.Console(2, this, "Setting video source to Virtual Device = 0");
                    _device.Control.ServerUrl.StringValue = "0.0.0.0";
                    return;
                }

                var result = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.VirtualDevice == value);
                
                if (result == null) return;

                Debug.Console(2, this, "Setting video source to Virtual Device = {0} | {1}", result.VirtualDevice, result.StreamUrl);

                _device.Control.ServerUrl.StringValue = result.StreamUrl;

                //set rx pairing
                _device.UsbInput.RemoteDeviceId.StringValue = result.LocalUsbId;
                _device.UsbInput.Pair();

                //set tx pairing
                result.RemoteUsbId = _device.UsbInput.LocalDeviceIdFeedback.StringValue;
                result.Pair();
            }
        }

        private void OnRouteUpdated()
        {
            Feedback.FireUpdate();
            var handler = RouteUpdated;

            if (handler == null) return;
            handler.Invoke(this, EventArgs.Empty);
        }
    }
}