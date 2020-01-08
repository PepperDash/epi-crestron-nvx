using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PepperDash.Core;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using Crestron.SimplSharp.Reflection;
using EssentialsExtensions;
using EssentialsExtensions.Attributes;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using NvxEpi.Interfaces;

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

        [Feedback(JoinNumber = 1, ValuePropertyName="Source")]
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

                OnRouteUpdated();
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
                    Debug.Console(2, this, "Video input is Virtual ID: {0}", result);
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
                    _device.Control.ServerUrl.StringValue = string.Empty;
                    return;
                }

                var result = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.VirtualDevice == value);
                
                if (result == null) return;

                Debug.Console(2, this, "Setting video source to Virtual Device = {0} | {1}", result.VirtualDevice, result.StreamUrl);

                _device.Control.ServerUrl.StringValue = result.StreamUrl;
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