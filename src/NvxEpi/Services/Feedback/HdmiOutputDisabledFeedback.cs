using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HdmiOutputDisabledFeedback
    {
        public const string Key = "HdmiOutDisabled";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new BoolFeedback(Key, () => false);

            var feedback = new BoolFeedback(Key, () => device.HdmiOut.DisabledByHdcpFeedback.BoolValue);
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiOutputEdidFeedback
    {
        public const string Key = "HdmiOutEdidManufacturer";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(Key, () => device.HdmiOut.ConnectedDevice.Manufacturer.StringValue);
            device.HdmiOut.ConnectedDevice.DeviceInformationChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}