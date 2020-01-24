using System;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core.Config;
using EssentialsExtensions.Attributes;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.DeviceHelpers
{
    public class NvxVideoWallHelper:NvxDeviceHelperBase
    {
        private readonly string _key;
        public override string Key
        {
            get { return string.Format("{0} {1}", _key, GetType().GetCType().Name); }
        }

        [Feedback(JoinNumber = 11, ValuePropertyName = "VideoWallMode")]
        public Feedback Feedback { get; set; }

        public int VideoWallMode
        {
            get { return _device.HdmiOut.VideoWallModeFeedback.UShortValue; }
            set { _device.HdmiOut.VideoWallMode.UShortValue = (ushort) value; }
        }

        public NvxVideoWallHelper(DeviceConfig config, DmNvxBaseClass device) : base(device)
        {
            _key = config.Key;
            Feedback = FeedbackFactory.GetFeedback(() => VideoWallMode);

            _device.HdmiOut.StreamChange += HdmiOutOnStreamChange;
            _device.BaseEvent += DeviceOnBaseEvent;
        }

        private void DeviceOnBaseEvent(GenericBase device, BaseEventArgs args)
        {
            switch (args.EventId)
            {
                case DMOutputEventIds.VideoWallModeFeedbackEventId:
                    Feedback.FireUpdate();
                    break;
            }
        }

        private void HdmiOutOnStreamChange(Stream stream, StreamEventArgs args)
        {
            switch (args.EventId)
            {
                case DMOutputEventIds.VideoWallModeFeedbackEventId:
                    Feedback.FireUpdate();
                    break;
            }
        }
    }
}