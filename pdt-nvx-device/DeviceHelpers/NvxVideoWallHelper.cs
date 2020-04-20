using System;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core.Config;
using EssentialsExtensions.Attributes;
using PepperDash.Essentials.Core;
using PepperDash.Core;

namespace NvxEpi.DeviceHelpers
{
    public class NvxVideoWallHelper : NvxDeviceHelperBase
    {
        private readonly string _key;
        public override string Key
        {
            get { return string.Format("{0} {1}", _key, GetType().GetCType().Name); }
        }

        public Feedback Feedback { get; set; }

        public int VideoWallMode
        {
            get { return _device.HdmiOut.VideoWallModeFeedback.UShortValue; }
            set { _device.HdmiOut.VideoWallMode.UShortValue = (ushort)value; }
        }

        public NvxVideoWallHelper(DeviceConfig config, DmNvxBaseClass device)
            : base(device)
        {
            _key = config.Key;
            Feedback = FeedbackFactory.GetFeedback(() => VideoWallMode);
            try
            {
                if (_device.HdmiOut == null)
                {
                    Debug.Console(0, "{0} does not have an HDMI Out. Skipping event subscription.", _device.ToString());
                    return;
                }

                _device.HdmiOut.StreamChange += HdmiOut_StreamChange;
            }
            catch (Exception ex)
            {
                Debug.LogError(Debug.ErrorLogLevel.Error, String.Format("Exception in VideoWallHelper: {0}", ex.Message));
            }
        }

        void HdmiOut_StreamChange(Crestron.SimplSharpPro.DeviceSupport.Stream stream, Crestron.SimplSharpPro.DeviceSupport.StreamEventArgs args)
        {
            switch (args.EventId)
            {
                case DMOutputEventIds.VideoWallModeFeedbackEventId:
                    Feedback.FireUpdate();
                    break;
                default:
                    Debug.Console(2, this, "Unhandled StreamEvent {0}", args.EventId);
                    break;
            }
        }
    }
}