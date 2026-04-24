using System;
using System.Threading.Tasks;
using NvxEpi.Devices;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Features.Routing;

public class NvxMockMatrixInput : IRoutingInputSlot
{
    private readonly NvxMockDevice device;

    public NvxMockMatrixInput(NvxMockDevice device)
        : base()
    {
        this.device = device;

        try
        {
            this.device.SyncDetected.OutputChange += (o, a) =>
                Task.Run(() =>
                {
                    try
                    {
                        VideoSyncChanged?.Invoke(this, new EventArgs());
                    }
                    catch (Exception ex)
                    {
                        this.LogError("Exception invoking VideoSyncChanged: {message}", ex.Message);
                        this.LogDebug(ex, "Stack Trace: ");
                    }
                });
        }
        catch (Exception ex)
        {
            this.LogError("Exception creating Matrix Input: {message}", ex.Message);
            this.LogDebug(ex, "Stack Trace: ");
        }
    }

    public string TxDeviceKey => device.Key;

    public int SlotNumber => device.DeviceId;

    public eRoutingSignalType SupportedSignalTypes =>
        eRoutingSignalType.AudioVideo | eRoutingSignalType.SecondaryAudio;

    public string Name => device.Name;

    public BoolFeedback IsOnline => device.IsOnline;

    public bool VideoSyncDetected => device.Sync;

    public string Key => $"{device.Key}";

    public event EventHandler VideoSyncChanged;
}
