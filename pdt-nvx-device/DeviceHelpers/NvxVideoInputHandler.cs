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
    public class NvxVideoInputHandler : NvxDeviceHelperBase, ISwitcher, IDynamicFeedback
    {
        private string _key;
        public override string Key
        {
            get { return string.Format("{0} {1}", _key, this.GetType().GetCType().Name); }
        }

        [Feedback(JoinNumber = 3, ValuePropertyName="Source")]
        public Feedback Feedback { get; set; }

        public NvxVideoInputHandler(DeviceConfig config, DmNvxBaseClass device)
            : base(device)
        {
            _key = config.Key;
            Feedback = FeedbackFactory.GetFeedback(() => Source);

            _device.BaseEvent += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.VideoSourceEventId:
                            OnRouteUpdated();
                            break;
                        case DMInputEventIds.MulticastAddressEventId:
                            OnRouteUpdated();
                            break;
                        default:
                            //Debug.Console(2, this, "Unhandled DM Input EventId {0}", args.EventId);
                            break;
                    }                      
                };

            _device.OnlineStatusChange += (sender, args) =>
                {
                    OnRouteUpdated();
                };
        }

        public string CurrentlyRouted
        {
            get { return _device.Control.VideoSource.ToString(); }
        }

        public int Source
        {
            get 
            {
                Debug.Console(2, this, "Video input source is {0}", _device.Control.VideoSourceFeedback); 
                return (int)_device.Control.VideoSourceFeedback; 
            }
            set 
            {
                _device.Control.VideoSource = (eSfpVideoSourceTypes)value;
            }
        }

        public event EventHandler RouteUpdated;

        private void OnRouteUpdated()
        {
            Feedback.FireUpdate();
            var handler = RouteUpdated;

            if (RouteUpdated == null) return;
            handler.Invoke(this, EventArgs.Empty);
        }
    }
}