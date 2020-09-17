﻿using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.DeviceFeedback
{
    public class IsStreamingVideo
    {
        public const string Key = "IsStreamingVideo";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, () => device.Control.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}