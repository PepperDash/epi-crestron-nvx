using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using PepperDash.Core;

namespace NvxEpi.Device.Services.Utilities
{
    public static class DebugExtensions
    {
        public static void RegisterForDeviceFeedback(this INvxDevice device)
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
                Debug.Console(2, device, "This firmware doesn't support NAX Audio Routing");
            }
            catch (Exception ex)
            {
                Debug.Console(2, device, "Exception Registering for Logging : {0}\r{1}", ex.Message, ex.StackTrace);
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
                    Debug.Console(2, keyed, "Received HDMI Stream Change Event ID:{0} from {1}", args.EventId,
                        input.NameFeedback.StringValue);
            }
        }

        private static void RegisterForHdmiOutputFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            if (device.HdmiOut == null)
                return;

            device.HdmiOut.StreamChange += (stream, args) =>
                Debug.Console(2, keyed, "Received HDMI Stream Change Event ID:{0} from {1}", args.EventId,
                    device.HdmiOut.NameFeedback.StringValue);

            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) =>
                Debug.Console(2, keyed, "Received Video Attributes Change:{0} from {1}", args.EventId,
                    device.HdmiOut.NameFeedback.StringValue);
        }

        private static void RegisterForNaxFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            try
            {
                if (device.DmNaxRouting == null)
                    return;

                device.DmNaxRouting.DmNaxRoutingChange += (stream, args) =>
                    Debug.Console(2, keyed, "Received NAX Routing Change Event ID:{0} from {1}", args.EventId,
                        device.HdmiOut.NameFeedback.StringValue);

                if (device.DmNaxRouting.DmNaxReceive != null)
                {
                    device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) =>
                        Debug.Console(2, keyed, "Recieved NAX Routing Receive Change:{0} from {1}", args.EventId,
                            device.HdmiOut.NameFeedback.StringValue);
                }

                if (device.DmNaxRouting.DmNaxTransmit != null)
                {
                    device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) =>
                        Debug.Console(2, keyed, "Recieved NAX Routing Transmit Change:{0}", args.EventId);
                }
            }
            catch (MissingMethodException ex)
            {
                Debug.Console(2, keyed, "This firmware doesn't support NAX Audio Routing : {0}\r{1}", ex.Message, ex.InnerException);
                throw;
            }
        }

        private static void RegisterForSecondaryAudioFeedback(DmNvxBaseClass device, IKeyed keyed)
        {
            try
            {
                if (device.SecondaryAudio == null)
                    return;

                device.SecondaryAudio.SecondaryAudioChange += (sender, args) =>
                    Debug.Console(2, keyed, "Received Secondary Audio Change Event ID:{0}", args.EventId);
            }
            catch (MissingMethodException ex)
            {
                Debug.Console(2, keyed, "This firmware doesn't support Secondary Audio Routing");
                throw;
            }
        }
    }
}