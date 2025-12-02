using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using System;
using System.Linq;

namespace NvxEpi.Features.Usbc.Input;

public class UsbcInput : UsbcInputBase
{
    public UsbcInput(INvx38XDeviceWithHardware device)
        : base(device)
    {
        try
        {
            if (device.Hardware is DmNvx38x)
            {

                Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is DmNvx38x", this);

                foreach (var inputNumber in device.Hardware.UsbcIn.Keys)
                {
                    try
                    {
                        var capability = UsbcHdcpCapabilityValueFeedback.GetFeedback(device.Hardware, inputNumber);

                        _capability.Add(inputNumber, capability);

                        var sync = UsbcSyncDetectedFeedback.GetFeedback(device.Hardware, inputNumber);

                        _sync.Add(inputNumber, sync);

                        var inputResolution = UsbcCurrentResolutionFeedback.GetFeedback(device.Hardware, inputNumber);

                        _currentResolution.Add(inputNumber, inputResolution);

                        var capabilityString = UsbcHdcpCapabilityFeedback.GetFeedback(device.Hardware, inputNumber);

                        _capabilityString.Add(inputNumber, capabilityString);

                        var audioChannels = UsbcAudioChannelsFeedback.GetFeedback(device.Hardware, inputNumber);

                        _audioChannels.Add(inputNumber, audioChannels);

                        var audioFormat = UsbcAudioFormatFeedback.GetFeedback(device.Hardware, inputNumber);

                        _audioFormat.Add(inputNumber, audioFormat);

                        var colorSpace = UsbcColorSpaceFeedback.GetFeedback(device.Hardware, inputNumber);

                        _colorSpace.Add(inputNumber, colorSpace);

                        var hdrType = UsbcHdrTypeFeedback.GetFeedback(device.Hardware, inputNumber);

                        _hdrType.Add(inputNumber, hdrType);

                        var hdcpSupport = UsbcHdcpSupportFeedback.GetFeedback(device.Hardware, inputNumber);

                        _hdcpSupport.Add(inputNumber, hdcpSupport);

                        Feedbacks.Add(hdcpSupport);
                        Feedbacks.Add(capability);
                        Feedbacks.Add(sync);
                        Feedbacks.Add(inputResolution);
                        Feedbacks.Add(capabilityString);
                        Feedbacks.Add(audioChannels);
                        Feedbacks.Add(audioFormat);
                        Feedbacks.Add(colorSpace);
                        Feedbacks.Add(hdrType);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogMessage(ex, "Exception getting information for Usbc {inputNumber}", this, inputNumber);
                    }
                }
            }
            else
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is NOT DmNvx38x", this);
            }

        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception getting Usbc Input information", this);
        }
    }
}
