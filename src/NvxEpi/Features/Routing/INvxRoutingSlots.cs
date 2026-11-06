using System;
using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

// Plugin-local replacement for the removed core IRoutingInputSlot/IRoutingOutputSlot -
// the slot abstraction was removed in the v3 routing overhaul with no core equivalent,
// and nothing outside this plugin consumed the old interfaces, so this carries only the
// members actually used by NvxGlobalRouter and its slot implementations. Extends the core
// IRoutingInputSlotInfo/IRoutingOutputSlotStatus so the mobile-control matrix messenger picks up
// each slot's online state, video sync, and tx/rx device key.
public interface INvxInputSlot : IRoutingInputSlotInfo
{
}

public interface INvxOutputSlot : IRoutingOutputSlotStatus
{
    Dictionary<eRoutingSignalType, INvxInputSlot> CurrentRoutes { get; }
}
