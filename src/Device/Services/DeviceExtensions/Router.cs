using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class Router
    {
        public static readonly string RouteOff = "$off";

        public static StringFeedback GetCurrentVideoRouteNameFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.CurrentVideoRouteName.ToString(),
                () =>
                {
                    if (device.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi1 ||
                        device.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi2)
                        return "No Source";

                    if (!device.Control.StartFeedback.BoolValue)
                        return "No Source";

                    var tx = DeviceManager
                        .AllDevices
                        .OfType<NvxDevice>()
                        .Where(t => t.IsTransmitter)
                        .FirstOrDefault(
                            x =>
                                x.StreamUrl.Equals(device.Control.ServerUrlFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return tx == null ? "No Source" : tx.Name;
                });

            device.BaseEvent += (@base, args) =>
            {
                if (args.EventId == DMInputEventIds.ServerUrlEventId ||
                    args.EventId == DMInputEventIds.StartEventId ||
                    args.EventId == DMInputEventIds.StopEventId ||
                    args.EventId == DMInputEventIds.PauseEventId ||
                    args.EventId == DMInputEventIds.VideoSourceEventId)
                    feedback.FireUpdate();
            };

            return feedback;
        }

        public static StringFeedback GetSecondaryAudioRouteNameFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.CurrentAudioRouteName.ToString(),
                () =>
                {
                    if (!device.SecondaryAudio.StartFeedback.BoolValue 
                        && device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.SecondaryStreamAudio)
                        return "No Source";

                    if (!device.Control.StartFeedback.BoolValue
                        && device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.PrimaryStreamAudio)
                        return "No Source";

                    if (device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input1
                        || device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input2)
                        return "No Source";

                    var tx = DeviceManager
                        .AllDevices
                        .OfType<NvxDevice>()
                        .Where(t => t.IsTransmitter)
                        .FirstOrDefault(
                            x =>
                                x.MulticastAudioAddress.Equals(device.SecondaryAudio.MulticastAddressFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return tx == null ? "No Source" : tx.Name;
                });

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            /*
            {
                if (device.Control.AudioSourceFeedback != DmNvxControl.eAudioSource.PrimaryStreamAudio)
                    return;

                if (args.EventId == DMInputEventIds.ServerUrlEventId ||
                    args.EventId == DMInputEventIds.StartEventId ||
                    args.EventId == DMInputEventIds.StopEventId ||
                    args.EventId == DMInputEventIds.PauseEventId)
                    
            };*/

            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => feedback.FireUpdate();
            /*{
                if (device.Control.AudioSourceFeedback != DmNvxControl.eAudioSource.SecondaryStreamAudio)
                    return;

                if (args.EventId == DMInputEventIds.MulticastAddressEventId ||
                    args.EventId == DMInputEventIds.StartEventId ||
                    args.EventId == DMInputEventIds.StopEventId ||
                    args.EventId == DMInputEventIds.PauseEventId ||
                    args.EventId == DMInputEventIds.AudioSourceEventId)
                    feedback.FireUpdate();
            };*/

            return feedback;
        }

        public static void RouteVideo(this DmNvxBaseClass device, string txName)
        {
            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                device.Control.Stop();
                return;
            }
               
            var tx = DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(t => t.IsTransmitter)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                device.Control.ServerUrl.StringValue = txByName.StreamUrl;           
                device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
                device.Control.Start();
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null) 
                return;

            device.Control.ServerUrl.StringValue = txByKey.StreamUrl;
            device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
            device.Control.Start();
        }

        public static void RouteSecondaryAudio(this DmNvxBaseClass device, string txName)
        {
            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                device.SecondaryAudio.Stop();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(t => t.IsTransmitter)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                device.SecondaryAudio.MulticastAddress.StringValue = String.IsNullOrEmpty(txByName.MulticastAudioAddress) 
                    ? txByName.MulticastAddress 
                    : txByName.MulticastAudioAddress;

                device.SecondaryAudio.Start();
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            device.SecondaryAudio.MulticastAddress.StringValue = String.IsNullOrEmpty(txByKey.MulticastAudioAddress)
                    ? txByKey.MulticastAddress
                    : txByKey.MulticastAudioAddress;

            device.SecondaryAudio.Start();
        }
    }
}