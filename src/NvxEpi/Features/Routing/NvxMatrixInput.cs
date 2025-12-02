using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.UsbcInput;
using NvxEpi.Devices;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using System;
using System.Linq;

namespace NvxEpi.Features.Routing;

public class NvxMatrixInput : IRoutingInputSlot
{
    private readonly NvxBaseDevice _device;

    public NvxMatrixInput(NvxBaseDevice device):base()
    {
        _device = device;
        
        try
        {
            if (_device is not IHdmiInput hdmiInput)
            {
                return;
            }

            if (hdmiInput.SyncDetected == null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Input is null", device);

                return;
            }

            foreach (var feedback in hdmiInput.SyncDetected)
            {
                if (feedback.Value == null)
                {
                    Debug.LogMessage(Serilog.Events.LogEventLevel.Warning, "Sync Feedback {feedbackKey} is null", device, feedback.Key);
                    continue;
                }

                feedback.Value.OutputChange += (o, a) => VideoSyncChanged?.Invoke(this, new EventArgs());
            }
        } catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception creating Matrix Input", device);
        }
        
    }        

    public string TxDeviceKey => _device.Key;

    public int SlotNumber => _device.DeviceId;

    public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.SecondaryAudio;

    public string Name => _device.Name;

    public BoolFeedback IsOnline => _device.IsOnline;

    public bool VideoSyncDetected => _device is IHdmiInput inputDevice && inputDevice.SyncDetected.Any(fb => fb.Value.BoolValue);

    public string Key => $"{_device.Key}";

    public event EventHandler VideoSyncChanged;
}
