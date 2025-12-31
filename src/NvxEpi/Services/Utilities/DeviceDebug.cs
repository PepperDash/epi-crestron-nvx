using System;
using System.Linq;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Utilities;

public static class DeviceDebug
{
    public static void RegisterForDeviceFeedback(INvxDeviceWithHardware device)
    {
        try
        {
            RegisterForHdmiInputFeedback(device.Hardware, device);
            RegisterForHdmiOutputFeedback(device.Hardware, device);
            RegisterForSecondaryAudioFeedback(device.Hardware, device);
            RegisterForNaxFeedback(device.Hardware, device);
        }
        catch (MissingMethodException ex)
        {
            device.LogError("Missing Method Exception Registering for Logging : {message}", ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception Registering for Logging : {message}", ex.Message);
            Debug.LogDebug(ex, "Stack trace: ");
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
                        feedback.LogVerbose("Received Update: '{value}'", args.BoolValue);

                    if (sender is IntFeedback)
                        feedback.LogVerbose("Received Update: '{value}'", args.IntValue);

                    if (sender is StringFeedback)
                        feedback.LogVerbose("Received Update: '{value}'", args.StringValue);
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
                keyed.LogVerbose(
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
            keyed.LogVerbose(
                "Received HDMI Stream Change Event ID:{0} from {1}",
                args.EventId,
                device.HdmiOut.NameFeedback.StringValue);

        device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) =>
            keyed.LogVerbose(
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
                keyed.LogVerbose(
                    "Received NAX Routing Change Event ID:{0}",
                    args.EventId);

            if (device.DmNaxRouting.DmNaxReceive != null)
            {
                device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) =>
                    keyed.LogVerbose(
                        "Recieved NAX Routing Receive Change:{0}",
                        args.EventId);
            }

            if (device.DmNaxRouting.DmNaxTransmit != null)
            {
                device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) =>
                    keyed.LogVerbose(
                        "Recieved NAX Routing Transmit Change:{0}",
                        args.EventId);
            }
        }
        catch (MissingMethodException ex)
        {
            keyed.LogError(
                "This firmware doesn't support NAX Audio Routing : {0}",
                ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            keyed.LogError(
                "This firmware doesn't support NAX Audio Routing : {0}",
                ex.Message);
            keyed.LogDebug(ex, "Stack Trace: ");
            throw;
        }
    }

    private static void RegisterForSecondaryAudioFeedback(DmNvxBaseClass device, IKeyed keyed)
    {
        try
        {
            if (device is not DmNvx35x)
                return;

            if (device.SecondaryAudio == null)
                return;

            device.SecondaryAudio.SecondaryAudioChange += (sender, args) =>
                keyed.LogVerbose("Received Secondary Audio Change Event ID:{0}", args.EventId);
        }
        catch (MissingMethodException ex)
        {
            keyed.LogError("This firmware doesn't support NAX Audio Routing : {0}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            keyed.LogError("This firmware doesn't support NAX Audio Routing : {0}", ex.Message);
            keyed.LogDebug(ex, "Stack Trace: ");
            throw;
        }
    }
}