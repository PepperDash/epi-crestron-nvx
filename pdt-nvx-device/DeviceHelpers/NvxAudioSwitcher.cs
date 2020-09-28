using System;
using System.Collections.Generic;
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
    public class NvxAudioSwitcher : NvxDeviceHelperBase, ISwitcher
    {
        private string _key;
        public override string Key
        {
            get { return _key; }
        }

        private int _selectedInput;

        public Feedback Feedback { get; set; }

        public event EventHandler RouteUpdated;

        public NvxAudioSwitcher(string key, DmNvxBaseClass device)
            : base(device)
        {
            _key = string.Format("{0} {1}", key, this.GetType().GetCType().Name);
            Feedback = FeedbackFactory.GetFeedback(() => Source);

            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.StreamUriFeedbackEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.SecondaryAudioModeFeedbackEventId:
                            Debug.Console(2, this, "Secondary audio mode: {0}", _device.SecondaryAudio.SecondaryAudioModeFeedback);
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.AudioSourceEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.StatusEventId:
                            Debug.Console(2, this, "StatusEventId {0}", _device.SecondaryAudio.StatusFeedback);
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.StopEventId:
                            Debug.Console(2, this, "StopEventId {0}", _device.SecondaryAudio.StatusFeedback);
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.StartEventId:
                            Debug.Console(2, this, "StartEventId {0}", _device.SecondaryAudio.StatusFeedback);
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.StatusTextEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.MulticastAddressEventId:
                            Debug.Console(2, this, "MulticastAddressEventId {0}", _device.SecondaryAudio.MulticastAddressFeedback);
                            OnRouteUpdated();
                            break;
                        default:
                            //Debug.Console(2, this, "Unhandled DM EventId {0}", args.EventId);
                            break;
                    };
                };
            }

            _device.OnlineStatusChange += (sender, args) =>
            {
                if (args.DeviceOnLine)
                {
                    //Source = _selectedInput;
                }

                OnRouteUpdated();
            };   
        }

        public string CurrentlyRouted
        {
            get
            {
                var result = "No Source";
                if (_isTransmitter) return _device.Control.AudioSourceFeedback.ToString();

                var device = _inputs
                    .FirstOrDefault(x => x.MulticastAudioAddress == _device.SecondaryAudio.MulticastAddressFeedback.StringValue);

                if (device != null && !string.IsNullOrEmpty(_device.SecondaryAudio.MulticastAddressFeedback.StringValue))
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

                var device = _inputs
                    .FirstOrDefault(x => x.MulticastAudioAddress == _device.SecondaryAudio.MulticastAddressFeedback.StringValue);
           
                if (device != null && !string.IsNullOrEmpty(_device.SecondaryAudio.MulticastAddressFeedback.StringValue))
                {
                    result = device.VirtualDevice;
                }
                
                Debug.Console(2, this, "Audio input is Virtual ID: {0}", result);
                return result;
            }
            set
            {
                Debug.Console(2, this, "Secondary audio status: {0}", _device.SecondaryAudio.StatusFeedback);
                Debug.Console(2, this, "Secondary audio mode: {0}", _device.SecondaryAudio.SecondaryAudioModeFeedback);

                if (_selectedInput != value) _selectedInput = value;
                if (_isTransmitter || !_device.IsOnline) return;

                Debug.Console(2, this, "Attempting to set the audio input to Virutal Input:{0}", value);
                if (value == 0)
                {
                    _device.SecondaryAudio.MulticastAddress.StringValue = string.Empty;
                    return;
                }

                var result = _inputs
                    .FirstOrDefault(x => x.VirtualDevice == value);

                if (result == null)
                {
                    Debug.Console(2, this, "Did not find a device at Virutal Input:{0}", value);
                    return;
                }

                Debug.Console(2, this, "Attemping to start the stream... Address: {0}", result.MulticastAudioAddress);
                _device.SecondaryAudio.MulticastAddress.StringValue = result.MulticastAudioAddress;
            }
        }

        protected void OnRouteUpdated()
        {
            Feedback.FireUpdate();
            var handler = RouteUpdated;

            if (handler == null) return;
            handler.Invoke(this, EventArgs.Empty);
        }

        #region ISwitcher Members


        public void SetInputs(IEnumerable<INvxDevice> inputs)
        {
            _inputs = inputs;
        }

        #endregion
    }
}