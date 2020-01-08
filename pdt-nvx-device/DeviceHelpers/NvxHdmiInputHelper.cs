﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PepperDash.Core;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using Crestron.SimplSharp.Reflection;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using NvxEpi.Interfaces;

namespace NvxEpi.DeviceHelpers
{
    public class NvxHdmiInputHelper : IKeyed, INvxHdmiInputHelper
    {
        Crestron.SimplSharpPro.DeviceSupport.HdmiInWithColorSpaceMode _input;

        private DmNvxBaseClass _device;

        private string _key;
        public string Key
        {
            get { return string.Format("{0} {1}", _key, _input.NameFeedback.StringValue); }
        }

        public Feedback SyncDetectedFb { get; set; }
        public Feedback HdmiCapabilityFb { get; set; }
        public Feedback HdmiSupportedLevelFb { get; set; }

        public NvxHdmiInputHelper(DeviceConfig config, Crestron.SimplSharpPro.DeviceSupport.HdmiInWithColorSpaceMode input, DmNvxBaseClass device)
        {
            _key = config.Key;
            _input = input;
            _device = device;

            SyncDetectedFb = FeedbackFactory.GetFeedback(() => SyncDetected);
            HdmiCapabilityFb = FeedbackFactory.GetFeedback(() => HdmiCapability);
            HdmiSupportedLevelFb = FeedbackFactory.GetFeedback(() => HdmiSupportedLevel);

            _input.StreamChange += (stream, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.SourceSyncEventId:
                            Debug.Console(2, this, "SyncDetectedEventId {0}", _input.SyncDetectedFeedback.BoolValue);
                            SyncDetectedFb.FireUpdate();
                            break;
                        case DMOutputEventIds.SyncDetectedEventId:
                            
                            break;
                        case DMInputEventIds.HdcpCapabilityFeedbackEventId:
                            HdmiCapabilityFb.FireUpdate();
                            break;
                        case DMInputEventIds.HdcpSupportedLevelFeedbackEventId:
                            HdmiSupportedLevelFb.FireUpdate();
                            break;
                        default:
                            Debug.Console(2, this, "Unhandled DM OutputEventId {0}", args.EventId);
                            break;
                    }
                };

            _device.OnlineStatusChange += (sender, args) =>
            {
                SyncDetectedFb.FireUpdate();
                HdmiCapabilityFb.FireUpdate();
                HdmiSupportedLevelFb.FireUpdate();
            };
        }

        public bool SyncDetected
        {
            get { return _input.SyncDetectedFeedback.BoolValue; }
        }
        public int HdmiCapability
        {
            get { return (int)_input.HdcpCapabilityFeedback; }
            set { _input.HdcpCapability = (eHdcpCapabilityType)value; }
        }
        public int HdmiSupportedLevel
        {
            get { return (int)_input.HdcpSupportedLevelFeedback; }
            set { _input.HdcpSupportedLevel = (eHdcpSupportedLevel)value; }
        }
    }
}