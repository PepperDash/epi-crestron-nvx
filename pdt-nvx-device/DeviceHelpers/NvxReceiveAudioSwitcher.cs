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
    public class NvxReceiveAudioSwitcher : NvxDeviceHelperBase, ISwitcher
    {
        private string _key;
        public override string Key
        {
            get { return string.Format("{0} {1}", _key, this.GetType().GetCType().Name); }
        }

        [Feedback(JoinNumber = 2, ValuePropertyName="Source")]
        public Feedback Feedback { get; set; }

        public event EventHandler RouteUpdated;

        public NvxReceiveAudioSwitcher(DeviceConfig config, DmNvxBaseClass device)
            : base(device)
        {
            _key = config.Key;
            Feedback = new IntFeedback(() => Source);

            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.StreamUriFeedbackEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.StatusEventId:
                            Debug.Console(2, this, "StatusEventId: {0}", _device.SecondaryAudio.StatusFeedback);
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
                        case DMInputEventIds.ReceiveAutomaticInitiationEnabledEventId:
                            Debug.Console(2, this, "ReceiveAutomaticInitiationEnabledEventId Update: {0}", _device.SecondaryAudio.ReceiveEnableAutomaticInitiationFeedback.BoolValue);
                            break;
                        case DMInputEventIds.ReceiveNameEventId:
                            Debug.Console(2, this, "ReceiveNameEventId {0}", _device.SecondaryAudio.ReceiveNameFeedback);
                            break;
                        case DMInputEventIds.SecondaryAudioModeFeedbackEventId:
                            Debug.Console(2, this, "Secondary Audio Mode Update: {0}", _device.SecondaryAudio.SecondaryAudioModeFeedback);
                            break;
                        case DMInputEventIds.MulticastAddressEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.ReceiveMulticastAddressEventId:
                            Debug.Console(2, this, "ReceiveMulticastAddressEventId: {0}", _device.SecondaryAudio.MulticastAddressFeedback.StringValue);
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.ReceiveStatusEventId:
                            Debug.Console(0, this, "ReceiveStatusEventId: {0}", _device.SecondaryAudio.ReceiveStatusFeedback.ToString());
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
                    Feedback.FireUpdate();
                    _device.SecondaryAudio.ReceiveEnableAutomaticInitiation();
                }
            };   
        }

        public string CurrentlyRouted
        {
            get
            {
                var result = "No Source";
                if (_isTransmitter) return "This is a TX";

                var device = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.MulticastAudioAddress == _device.SecondaryAudio.MulticastAddressFeedback.StringValue);

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

                Debug.Console(0, this, "Receive status = {0}", _device.SecondaryAudio.ReceiveStatusFeedback.ToString());
                Debug.Console(0, this, "Multicast Address = {0}", _device.SecondaryAudio.MulticastAddressFeedback.StringValue);

                if (_device.SecondaryAudio.ReceiveStatusFeedback != DmNvxBaseClass.DmNvx35xSecondaryAudio.eDeviceStatus.StreamStarted)
                {
                    Debug.Console(2, this, "The audio stream is stopped...");
                    return 0;
                }

                var device = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.MulticastAudioAddress == _device.SecondaryAudio.ReceiveMulticastAddressFeedback.StringValue);
           
                if (device != null)
                {
                    result = device.VirtualDevice;
                }
                
                Debug.Console(2, this, "Audio input is Virtual ID: {0}: {0}", result);
                return result;
            }
            set
            {
                if (_isTransmitter) return;
                
                if (value == 0)
                {
                    _device.SecondaryAudio.ReceiveStop();
                    Debug.Console(2, this, "Stopping the audio stream...");
                    return;
                }

                _device.SecondaryAudio.ReceiveStart();

                var result = NvxDeviceEpi.Transmitters
                    .FirstOrDefault(x => x.VirtualDevice == value);

                if (result == null) return;

                Debug.Console(2, this, "Attemping to start the stream... Address: {0}", result.MulticastAudioAddress);
                _device.SecondaryAudio.ReceiveMulticastAddress.StringValue = result.MulticastAudioAddress;
            }
        }

        protected void OnRouteUpdated()
        {
            Feedback.FireUpdate();
            var handler = RouteUpdated;

            if (handler == null) return;
            handler.Invoke(this, EventArgs.Empty);
        }
    }
}