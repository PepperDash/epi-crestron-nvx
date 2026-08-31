using System;
using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

// Plugin-local replacement for the removed core IRoutingInputSlot/IRoutingOutputSlot -
// the slot abstraction was removed in the v3 routing overhaul with no core equivalent,
// and nothing outside this plugin consumed the old interfaces, so this carries only the
// members actually used by NvxGlobalRouter and its slot implementations. Extends the core
// IRoutingSlotInfo/IRoutingOutputSlotInfo so NvxGlobalRouter can expose IHasNamedRoutingSlots.
public interface INvxInputSlot : IRoutingSlotInfo
{
    string TxDeviceKey { get; }
    BoolFeedback IsOnline { get; }
    bool VideoSyncDetected { get; }
    event EventHandler VideoSyncChanged;
}

public interface INvxOutputSlot : IRoutingOutputSlotInfo
{
    string RxDeviceKey { get; }
    Dictionary<eRoutingSignalType, INvxInputSlot> CurrentRoutes { get; }
}
