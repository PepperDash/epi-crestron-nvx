using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class MulticastAddress
    {
        public static StringFeedback GetMulticastAddressFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.MulticastAddress.ToString(),
                () => device.Control.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        /*public static DmNvxBaseClass SetMulticastAddress(this DmNvxBaseClass device, string address)
        {
            if (device is DmNvxD3x || String.IsNullOrEmpty(address))
                return device;

            device.Control.MulticastAddress.StringValue = address;
            return device;
        }

        public static DmNvxBaseClass SetNaxTxAddress(this DmNvxBaseClass device, string address)
        {
            if (String.IsNullOrEmpty(address))
                return device;

            device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = address;
            return device;
        }

        public static DmNvxBaseClass SetNaxRxAddress(this DmNvxBaseClass device, string address)
        {
            if (String.IsNullOrEmpty(address))
                return device;

            device.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
            return device;
        }*/

        public static StringFeedback GetNaxTxMulticastAddressFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.NaxTxAddress.ToString(),
                () => device.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue);

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetNaxRxMulticastAddressFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.NaxRxAddress.ToString(),
                () => device.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue);

            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}