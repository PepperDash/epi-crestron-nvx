using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class UsbRemoteAddressFeedback
    {
        public const string Key = "UsbRemoteId";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.UsbInput == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(Key, () => device.UsbInput.RemoteDeviceIdFeedback.StringValue);

            device.UsbInput.UsbInputChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static ReadOnlyDictionary<uint, StringFeedback> GetFeedbacks(DmNvxBaseClass device)
        {
            var dict = new Dictionary<uint, StringFeedback>();

            if (device.UsbInput != null)
            {
                for (uint i = 0; i <= device.UsbInput.RemoteDeviceIdFeedbacks.Count; i++)
                {
                    StringOutputSig sig;
                    if (!device.UsbInput.RemoteDeviceIdFeedbacks.TryGetValue(i, out sig))
                        continue;

                    dict.Add(i, new StringFeedback(Key + "-" + i, () => sig.StringValue));
                }

                device.UsbInput.UsbInputChange += (sender, args) => dict.Values.ToList().ForEach(x => x.FireUpdate());
            }

            return new ReadOnlyDictionary<uint, StringFeedback>(dict);
        }
    }
}