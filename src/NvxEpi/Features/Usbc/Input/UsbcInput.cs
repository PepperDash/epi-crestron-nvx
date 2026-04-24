using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Core;

namespace NvxEpi.Features.Usbc.Input;

public class UsbcInput : UsbcInputBase
{
    public UsbcInput(INvxDeviceWithHardware device)
        : base(device)
    {
        // try
        // {
        //     if (device.Hardware is DmNvxE760x)
        //     {

        //         Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is DmNvxE760x", this);
        //         var capability = DmHdcpCapabilityValueFeedback.GetFeedback(device.Hardware);

        //         var sync = DmSyncDetectedFeedback.GetFeedback(device.Hardware);

        //         _capability.Add(1, capability);
        //         _sync.Add(1, sync);

        //         Feedbacks.Add(capability);
        //         Feedbacks.Add(sync);

        //         return;
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogMessage(ex, "Exception getting DmNVXE760x information", this);
        // }

        try
        {
            //Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is NOT DmNvxE760x", this);
            if (device.Hardware is not DmNvx38x nvx38x)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Warning, "Hardware is NOT DmNvx38x and does not support UsbcIn", this);
                return;
            }

            if (nvx38x.UsbcIn == null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Warning, "UsbcIn is null on this DmNvx38x device", this);
                return;
            }

            foreach (var inputNumber in nvx38x.UsbcIn.Keys)
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
                    Debug.LogMessage(ex, "Exception getting information for USBC {inputNumber}", this, inputNumber);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception getting USBC Input information", this);
        }
    }
}
