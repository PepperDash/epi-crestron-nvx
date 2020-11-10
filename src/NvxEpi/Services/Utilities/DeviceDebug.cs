using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Utilities
{
    public class DeviceDebug
    {
        public static void RegisterForDeviceFeedback(INvxHardware device)
        {
            try
            {
                device.Hardware.BaseEvent += (sender, args) =>
                    Debug.Console(2, device, "Received Base Event:{0}", args.EventId);

                RegisterForHdmiInputFeedback(device.Hardware, device);
                RegisterForHdmiOutputFeedback(device.Hardware, device);
                RegisterForSecondaryAudioFeedback(device.Hardware, device);
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
            foreach (var item in feedback.Feedbacks)
            {
                item.OutputChange += (sender, args) =>
                    {
                        var keyed = sender as IKeyed;
                        if (keyed == null || String.IsNullOrEmpty(keyed.Key))
                            return;

                        if (sender is BoolFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", keyed.Key, args.BoolValue);

                        if (sender is IntFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", keyed.Key, args.IntValue);

                        if (sender is StringFeedback)
                            Debug.Console(1, feedback, "Received {0} Update : '{1}'", keyed.Key, args.StringValue);
                    };
            }
        }

        public static void RegisterForRoutingInputPortFeedback(IRoutingInputs device)
        {
            foreach (var item in device.InputPorts.OfType<RoutingInputPortWithVideoStatuses>())
            {
                var keyed = item;
                item.VideoStatus.HdcpActiveFeedback.OutputChange +=
                    (sender, args) => PrintRoutingInputFeedbackInfo(keyed, sender, args);

                item.VideoStatus.HdcpStateFeedback.OutputChange +=
                    (sender, args) => PrintRoutingInputFeedbackInfo(keyed, sender, args);

                item.VideoStatus.VideoSyncFeedback.OutputChange +=
                    (sender, args) => PrintRoutingInputFeedbackInfo(keyed, sender, args);

                item.VideoStatus.VideoResolutionFeedback.OutputChange +=
                    (sender, args) => PrintRoutingInputFeedbackInfo(keyed, sender, args);
            }
        }

        public static void RegisterForRoutingOutputFeedback(IRoutingOutputs device)
        {
            foreach (var item in device.OutputPorts)
            {
                var port = item;
                item.InUseTracker.InUseFeedback.OutputChange +=
                    (sender, args) => Debug.Console(1, device, "Ouput '{0}' in use: {1}", port.Key, args.BoolValue);
            }
        }

        private static void PrintRoutingInputFeedbackInfo(IKeyed port, object sender, FeedbackEventArgs args)
        {
            var keyed = sender as IKeyed;
            if (keyed == null || String.IsNullOrEmpty(keyed.Key))
                return;

            if (sender is BoolFeedback)
                Debug.Console(1, port, "Received {0} Update : '{1}'", keyed.Key, args.BoolValue);

            if (sender is IntFeedback)
                Debug.Console(1, port, "Received {0} Update : '{1}'", keyed.Key, args.IntValue);

            if (sender is StringFeedback)
                Debug.Console(1, port, "Received {0} Update : '{1}'", keyed.Key, args.StringValue);
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
                        "Received NAX Routing Change Event ID:{0} from {1}",
                        args.EventId,
                        device.HdmiOut.NameFeedback.StringValue);

                if (device.DmNaxRouting.DmNaxReceive != null)
                {
                    device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) =>
                        Debug.Console(2,
                            keyed,
                            "Recieved NAX Routing Receive Change:{0} from {1}",
                            args.EventId,
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