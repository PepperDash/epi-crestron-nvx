using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Devices;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using System.Linq;

namespace NvxEpi.Extensions;
public static class RoutingFeedbackExtensions
{
    public static RouteSwitchDescriptor HandleBaseEvent(this IRoutingWithFeedback parent, BaseEventArgs args)
    {
        if (parent is not NvxBaseDevice parentBase)
        {
            return null;
        }

        switch (args.EventId)
        {
            case DMInputEventIds.VideoSourceEventId: // Video source for HDMI Output has changed
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Handling Video source update for {parent}", parent, parent.Key);

                    var currentVideoInput = parentBase.Hardware.Control.VideoSourceFeedback;

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "currentVideoInput: {currentVideoInput}", parent, currentVideoInput.ToString());

                    var currentInputPort = parent.InputPorts.FirstOrDefault((ip) =>
                    {
                        if (ip.FeedbackMatchObject is not eSfpVideoSourceTypes matchObject) { return false; }

                        return matchObject == currentVideoInput;
                    });

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Found input port {currentInputPort} for {input}", parent, currentInputPort, currentVideoInput);

                    if (currentInputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for {currentSource}", parent, currentVideoInput);
                    }

                    RoutingOutputPort outputPort;

                    if (!parentBase.IsTransmitter)
                    {
                        outputPort = parent.OutputPorts.FirstOrDefault((op) => op.Key == SwitcherForHdmiOutput.Key);
                    }
                    else
                    {
                        outputPort = parent.OutputPorts.FirstOrDefault((op) => op.Key == SwitcherForStreamOutput.Key);
                    }

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Found output port {outputPort} for {input}", parent, outputPort, currentVideoInput);

                    if (outputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No output port found for {key}", parent, SwitcherForHdmiOutput.Key);
                        return null;
                    }

                    var existingRouteDescriptor = parent.CurrentRoutes.FirstOrDefault((rd) => rd.OutputPort.Key == outputPort.Key);

                    if (existingRouteDescriptor == null && currentInputPort != null)
                    {
                        var newRouteDescriptor = new RouteSwitchDescriptor(outputPort, currentInputPort);
                        parent.CurrentRoutes.Add(newRouteDescriptor);

                        return newRouteDescriptor;
                    }
                    
                    if(currentInputPort == null)
                    {
                        parent.CurrentRoutes.Remove(existingRouteDescriptor);
                        return null;
                    }

                    existingRouteDescriptor.InputPort = currentInputPort;

                    return existingRouteDescriptor;                    
                }
            case DMInputEventIds.DmNaxAudioSourceFeedbackEventId: //Analog/HDMI Output Audio source
                {
                    var currentNaxAudioInput = parentBase.Hardware.Control.DmNaxAudioSourceFeedback;

                    var currentInputPort = parent.InputPorts.FirstOrDefault((ip) =>
                    {
                        if (ip.FeedbackMatchObject is not DmNvxControl.eAudioSource matchObject) { return false; }

                        return matchObject == currentNaxAudioInput;
                    });

                    if (currentInputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for {currentSource}", parent, currentNaxAudioInput);
                        return null;
                    }

                    var outputPort = parent.OutputPorts.FirstOrDefault((op) => op.Key == SwitcherForAnalogAudioOutput.Key);

                    if (outputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No output port found for {key}", parent, SwitcherForSecondaryAudioOutput.Key);
                        return null;
                    }

                    var existingRouteDescriptor = parent.CurrentRoutes.FirstOrDefault((rd) => rd.OutputPort.Key == outputPort.Key);

                    if (existingRouteDescriptor == null && currentInputPort != null)
                    {
                        var newRouteDescriptor = new RouteSwitchDescriptor(outputPort, currentInputPort);
                        parent.CurrentRoutes.Add(newRouteDescriptor);

                        return newRouteDescriptor;
                    }

                    if (currentInputPort == null)
                    {
                        parent.CurrentRoutes.Remove(existingRouteDescriptor);
                        return null;
                    }

                    existingRouteDescriptor.InputPort = currentInputPort;
                    return existingRouteDescriptor;
                }
            case DMInputEventIds.AudioSourceEventId: //Analog/HDMI Output Audio source
                {
                    var currentAnalogAudioInput = parentBase.Hardware.Control.AudioSourceFeedback;

                    var currentInputPort = parent.InputPorts.FirstOrDefault((ip) =>
                    {
                        if (ip.FeedbackMatchObject is not DmNvxControl.eAudioSource matchObject) { return false; }

                        return matchObject == currentAnalogAudioInput;
                    });

                    if (currentInputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for {currentSource}", parent, currentAnalogAudioInput);
                        return null;
                    }

                    var outputPort = parent.OutputPorts.FirstOrDefault((op) => op.Key == SwitcherForAnalogAudioOutput.Key);

                    if (outputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No output port found for {key}", parent, SwitcherForHdmiOutput.Key);
                        return null;
                    }

                    var existingRouteDescriptor = parent.CurrentRoutes.FirstOrDefault((rd) => rd.OutputPort.Key == outputPort.Key);

                    if (existingRouteDescriptor == null && currentInputPort != null)
                    {
                        var newRouteDescriptor = new RouteSwitchDescriptor(outputPort, currentInputPort);
                        parent.CurrentRoutes.Add(newRouteDescriptor);

                        return newRouteDescriptor;
                    }

                    if (currentInputPort == null)
                    {
                        parent.CurrentRoutes.Remove(existingRouteDescriptor);
                        return null;
                    }

                    existingRouteDescriptor.InputPort = currentInputPort;
                    return existingRouteDescriptor;
                }
            default:
                return null;
        }        
    }
}
