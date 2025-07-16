using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using System;
using System.Linq;

namespace NvxEpi.Features.Dm.Input;

public class DmInput : DmInputBase
{        
    public DmInput(INvxDeviceWithHardware device)
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
        } catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception getting DmNVXE760x information", this);
        }
    }
}
