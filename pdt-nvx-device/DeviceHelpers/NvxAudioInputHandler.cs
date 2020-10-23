using System;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Interfaces;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.DeviceHelpers
{
    public class NvxAudioInputHandler : NvxDeviceHelperBase, ISwitcher
    {
        private readonly string _key;
        public override string Key
        {
            get { return _key; }
        }

        public Feedback Feedback { get; set; }

        public NvxAudioInputHandler(string key, DmNvxBaseClass device)
            : base(device)
        {
            _key = string.Format("{0} {1}", key, this.GetType().GetCType().Name);
            Feedback = FeedbackFactory.GetFeedback(() => Source);

            _device.BaseEvent += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.AudioSourceEventId:
                            OnRouteUpdated();
                            break;
                        default:
                            //Debug.Console(2, this, "Unhandled DM Input EventId {0}", args.EventId);
                            break;
                    }; 
                };
        }

        public string CurrentlyRouted
        {
            get { return _device.Control.AudioSourceFeedback.ToString(); }
        }

        public int Source 
        {
            get
            {
                Debug.Console(2, this, "Audio input source is {0}", _device.Control.AudioSourceFeedback); 
                return (int)_device.Control.AudioSourceFeedback;
            }
            set
            {
                _device.Control.AudioSource = (DmNvxControl.eAudioSource)value;
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

        #region ISwitcher Members


        public void SetInputs(System.Collections.Generic.IEnumerable<INvxDevice> inputs)
        {
            _inputs = inputs;
        }

        #endregion
    }
}