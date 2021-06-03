using System;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class UsbStatusFeedback
    {
        public const string Key = "UsbStatus";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.UsbInput == null)
                throw new NotSupportedException(device.GetType().GetCType().Name);

            var feedback = new StringFeedback(Key, () => device.UsbInput.StatusFeedback.ToString());

            device.UsbInput.UsbInputChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}