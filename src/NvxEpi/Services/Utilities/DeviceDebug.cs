using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Utilities
{
    public static class DeviceDebug
    {
        public static void RegisterForDeviceFeedback(INvxDeviceWithHardware device)
        {
            try
            {
                device.Hardware.BaseEvent += (sender, args) =>
                    Debug.Console(2, device, "Received Base Event:{0}", args.EventId);

                RegisterForHdmiInputFeedback(device.Hardware, device);
                RegisterForHdmiOutputFeedback(device.Hardware, device);
                RegisterForSecondaryAudioFeedback(device.Hardware, device);
                RegisterForNaxFeedback(device.Hardware, device);
            }
            catch (MissingMethodException ex)
            {
                Debug.Console(2,
                    device,
                    "Missing Method Exception Registering for Logging : {0}\r{1}",
                    ex.Message,
                    ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.Console(2, device, "Exception Registering for Logging : {0}\r{1}", ex.Message, ex.StackTrace);
            }
        }

        public static void RegisterForPluginFeedback(IHasFeedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException("IHasFeedback");

            if (feedback.Feedbacks == null)
                throw new NullReferenceException("Feedbacks");

            foreach (var item in feedback.Feedbacks.Where(x => x != null && !string.IsNullOrEmpty(x.Key)))
            {
                var fb = item;
                item.OutputChange += (sender, args) =>
                    {
                        if (sender is BoolFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", fb.Key, args.BoolValue);

                        if (sender is IntFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", fb.Key, args.IntValue);

                        if (sender is StringFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", fb.Key, args.StringValue);
                    };
            }
        }

        private static void RegisterForHdmiInputFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            if (device.HdmiIn == null)
                return;

            foreach (var item in device.HdmiIn)
            {
                var input = item;
                input.StreamChange += (stream, args) =>
                    Debug.Console(2,
                        keyed,
                        "Received HDMI Stream Change Event ID:{0} from {1}",
                        args.EventId,
                        input.NameFeedback.StringValue);
            }
        }

        private static void RegisterForHdmiOutputFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            if (device.HdmiOut == null)
                return;

            device.HdmiOut.StreamChange += (stream, args) =>
                Debug.Console(2,
                    keyed,
                    "Received HDMI Stream Change Event ID:{0} from {1}",
                    args.EventId,
                    device.HdmiOut.NameFeedback.StringValue);

            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) =>
                Debug.Console(2,
                    keyed,
                    "Received Video Attributes Change:{0} from {1}",
                    args.EventId,
                    device.HdmiOut.NameFeedback.StringValue);
        }

        private static void RegisterForNaxFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            try
            {
                if (device.DmNaxRouting == null)
                    return;

                device.DmNaxRouting.DmNaxRoutingChange += (stream, args) =>
                    Debug.Console(2,
                        keyed,
                        "Received NAX Routing Change Event ID:{0}",
                        args.EventId);

                if (device.DmNaxRouting.DmNaxReceive != null)
                {
                    device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) =>
                        Debug.Console(2,
                            keyed,
                            "Recieved NAX Routing Receive Change:{0}",
                            args.EventId);
                }

                if (device.DmNaxRouting.DmNaxTransmit != null)
                {
                    device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) =>
                        Debug.Console(2, keyed, "Recieved NAX Routing Transmit Change:{0}", args.EventId);
                }
            }
            catch (MissingMethodException ex)
            {
                Debug.Console(2,
                    keyed,
                    "This firmware doesn't support NAX Audio Routing : {0}\r{1}",
                    ex.Message,
                    ex.InnerException);
                throw;
            }
            catch (Exception ex)
            {
                Debug.Console(2,
                    keyed,
                    "This firmware doesn't support NAX Audio Routing : {0}\r{1}",
                    ex.Message,
                    ex.InnerException);
                throw;
            }
        }

        private static void RegisterForSecondaryAudioFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            try
            {
                if (!( device is DmNvx35x ))
                    return;

                if (device.SecondaryAudio == null)
                    return;

                device.SecondaryAudio.SecondaryAudioChange += (sender, args) =>
                    Debug.Console(2, keyed, "Received Secondary Audio Change Event ID:{0}", args.EventId);
            }
            catch (MissingMethodException ex)
            {
                Debug.Console(2,
                    keyed,
                    "This firmware doesn't support NAX Audio Routing : {0}\r{1}",
                    ex.Message,
                    ex.InnerException);
                throw;
            }
            catch (Exception ex)
            {
                Debug.Console(2,
                    keyed,
                    "This firmware doesn't support NAX Audio Routing : {0}\r{1}",
                    ex.Message,
                    ex.InnerException);
                throw;
            }
        }
    }
}