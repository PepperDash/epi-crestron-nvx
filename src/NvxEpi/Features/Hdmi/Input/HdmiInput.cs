using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Core;

namespace NvxEpi.Features.Hdmi.Input;

public class HdmiInput : HdmiInputBase
{
    public HdmiInput(INvxDeviceWithHardware device)
        : base(device)
    {
        try
        {
            if (device.Hardware is DmNvxE760x)
            {

                Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is DmNvxE760x", this);
                var capability = DmHdcpCapabilityValueFeedback.GetFeedback(device.Hardware);

                var sync = DmSyncDetectedFeedback.GetFeedback(device.Hardware);

                _capability.Add(1, capability);
                _sync.Add(1, sync);

                Feedbacks.Add(capability);
                Feedbacks.Add(sync);

                return;
            }
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception getting DmNVXE760x information", this);
        }

        try
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Hardware is NOT DmNvxE760x", this);

            foreach (var inputNumber in device.Hardware.HdmiIn.Keys)
            {
                try
                {
                    var capability = HdmiHdcpCapabilityValueFeedback.GetFeedback(device.Hardware, inputNumber);

                    _capability.Add(inputNumber, capability);

                    var sync = HdmiSyncDetectedFeedback.GetFeedback(device.Hardware, inputNumber);

                    _sync.Add(inputNumber, sync);

                    var inputResolution = HdmiCurrentResolutionFeedback.GetFeedback(device.Hardware, inputNumber);

                    _currentResolution.Add(inputNumber, inputResolution);

                    var capabilityString = HdmiHdcpCapabilityFeedback.GetFeedback(device.Hardware, inputNumber);

                    _capabilityString.Add(inputNumber, capabilityString);

                    var audioChannels = HdmiAudioChannelsFeedback.GetFeedback(device.Hardware, inputNumber);

                    _audioChannels.Add(inputNumber, audioChannels);

                    var audioFormat = HdmiAudioFormatFeedback.GetFeedback(device.Hardware, inputNumber);

                    _audioFormat.Add(inputNumber, audioFormat);

                    var colorSpace = HdmiColorSpaceFeedback.GetFeedback(device.Hardware, inputNumber);

                    _colorSpace.Add(inputNumber, colorSpace);

                    var hdrType = HdmiHdrTypeFeedback.GetFeedback(device.Hardware, inputNumber);

                    _hdrType.Add(inputNumber, hdrType);

                    var hdcpSupport = HdmiHdcpSupportFeedback.GetFeedback(device.Hardware, inputNumber);

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
                    Debug.LogMessage(ex, "Exception getting information for HDMI {inputNumber}", this, inputNumber);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception getting HDMI Input information", this);
        }
    }
}
