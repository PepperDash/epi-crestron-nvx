﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class UsbModeFeedback
    {
        public const string Key = "UsbMode";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new StringFeedback(Key, () => device.UsbInput.ModeFeedback.ToString());
            device.UsbInput.UsbInputChange += (sender, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}