using System;
using System.Linq;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class CurrentRoute
    {
        public static StringFeedback GetCurrentVideoRouteNameFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.CurrentVideoRouteName.ToString(),
                () =>
                {
                    if (device.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi1 ||
                        device.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi2 ||
                        device.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                        return NvxRouter.Instance.NoSourceText;

                    if (!device.Control.StartFeedback.BoolValue)
                        return NvxRouter.Instance.NoSourceText;

                    if (String.IsNullOrEmpty(device.Control.ServerUrl.StringValue))
                        return NvxRouter.Instance.NoSourceText;

                    var tx = DeviceManager
                        .AllDevices
                        .OfType<NvxDevice>()
                        .Where(t => t.IsTransmitter)
                        .FirstOrDefault(
                            x =>
                                x.StreamUrl.Equals(device.Control.ServerUrlFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return tx == null ? NvxRouter.Instance.NoSourceText : tx.Name;
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
                        return NvxRouter.Instance.NoSourceText;

                    if (!device.Control.StartFeedback.BoolValue
                        && device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.PrimaryStreamAudio)
                        return NvxRouter.Instance.NoSourceText;

                    if (device.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Disable
                        && device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.PrimaryStreamAudio)
                        return NvxRouter.Instance.NoSourceText;

                    if (device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input1
                        || device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input2)
                        return NvxRouter.Instance.NoSourceText;

                    if (device.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.SecondaryStreamAudio)
                    {
                        if (String.IsNullOrEmpty(device.SecondaryAudio.MulticastAddressFeedback.StringValue))
                            return NvxRouter.Instance.NoSourceText;

                        var tx = DeviceManager
                            .AllDevices
                            .OfType<NvxDevice>()
                            .Where(t => t.IsTransmitter)
                            .FirstOrDefault(
                                x =>
                                    x.MulticastAudioAddress.Equals(
                                        device.SecondaryAudio.MulticastAddressFeedback.StringValue,
                                        StringComparison.OrdinalIgnoreCase));

                        return tx == null ? NvxRouter.Instance.NoSourceText : tx.Name;
                    }

                    if (String.IsNullOrEmpty(device.Control.ServerUrl.StringValue))
                        return NvxRouter.Instance.NoSourceText;

                    var result = DeviceManager
                        .AllDevices
                        .OfType<NvxDevice>()
                        .Where(t => t.IsTransmitter)
                        .FirstOrDefault(
                            x =>
                                x.StreamUrl.Equals(
                                    device.Control.ServerUrl.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return result == null ? NvxRouter.Instance.NoSourceText : result.Name;
                });

            device.BaseEvent += (@base, args) => // feedback.FireUpdate();
            {
                if (args.EventId == DMInputEventIds.ServerUrlEventId ||
                    args.EventId == DMInputEventIds.StartEventId ||
                    args.EventId == DMInputEventIds.StopEventId ||
                    args.EventId == DMInputEventIds.PauseEventId ||
                    args.EventId == DMInputEventIds.AudioSourceEventId ||
                    args.EventId == DMInputEventIds.ActiveAudioSourceEventId ||
                    args.EventId == DMInputEventIds.StatusEventId)
                    feedback.FireUpdate();
            };

            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => //feedback.FireUpdate();
            {
                if (args.EventId == DMInputEventIds.MulticastAddressEventId ||
                    args.EventId == DMInputEventIds.StartEventId ||
                    args.EventId == DMInputEventIds.StopEventId ||
                    args.EventId == DMInputEventIds.PauseEventId ||
                    args.EventId == DMInputEventIds.AudioSourceEventId ||
                    args.EventId == DMInputEventIds.ActiveAudioSourceEventId ||
                    args.EventId == DMInputEventIds.StatusEventId)
                    feedback.FireUpdate();
            };

            return feedback;
        }
    }
}