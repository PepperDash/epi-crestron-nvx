using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

/// <summary>
/// Matrix output slot for a receiver <see cref="NvxMockDevice"/> with includeInMatrixRouting, so a bench system
/// without NVX hardware lists the mock as a routing destination. The route shown follows the mock's
/// CurrentVideoRouteValue (the routed transmitter's DeviceId), set when the primary stream router hands it a stream.
/// </summary>
public class NvxMockMatrixOutput : INvxOutputSlot
{
    private readonly NvxMockDevice device;

    public NvxMockMatrixOutput(NvxMockDevice device)
    {
        this.device = device;

        try
        {
            // Mocks have no separate secondary audio route, so audio follows video.
            this.device.CurrentVideoRouteValue.OutputChange += (o, args) =>
            {
                var inputSlot = NvxGlobalRouter.Instance.InputSlots.Values.FirstOrDefault(input =>
                    input.SlotNumber == args.IntValue
                );

                this.LogVerbose("Video: Found input slot {inputSlot} for {inputNumber}", inputSlot?.Key ?? "null", args.IntValue);

                currentRoutes[eRoutingSignalType.Video] = inputSlot;
                currentRoutes[eRoutingSignalType.Audio] = inputSlot;

                OutputSlotChanged?.Invoke(this, new EventArgs());
            };
        }
        catch (Exception ex)
        {
            this.LogError("Exception creating Mock Matrix Output: {message}", ex.Message);
            this.LogDebug(ex, "Stack Trace: ");
        }
    }

    public NvxMockDevice Device => device;

    public string RxDeviceKey => device.Key;

    public BoolFeedback IsOnline => device.IsOnline;

    private readonly Dictionary<eRoutingSignalType, INvxInputSlot> currentRoutes = new()
    {
        { eRoutingSignalType.Audio, default },
        { eRoutingSignalType.Video, default },
        { eRoutingSignalType.Usb, default },
    };

    public Dictionary<eRoutingSignalType, INvxInputSlot> CurrentRoutes => currentRoutes;

    // IRoutingOutputSlotInfo view of CurrentRoutes - input slot key per signal type.
    public IReadOnlyDictionary<eRoutingSignalType, string> CurrentRouteInputKeys =>
        currentRoutes.Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Key);

    public int SlotNumber => device.DeviceId;

    public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo;

    public string Name => device.Name;

    public string Key => $"{device.Key}";

    public bool IsEnabled => device.IsEnabled;

    public event EventHandler OutputSlotChanged;
}
