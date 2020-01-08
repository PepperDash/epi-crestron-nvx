using System;
namespace NvxEpi.Interfaces
{
    public interface INvxHdmiInputHelper
    {
        int HdmiCapability { get; set; }
        PepperDash.Essentials.Core.Feedback HdmiCapabilityFb { get; set; }
        int HdmiSupportedLevel { get; set; }
        PepperDash.Essentials.Core.Feedback HdmiSupportedLevelFb { get; set; }
        bool SyncDetected { get; }
        PepperDash.Essentials.Core.Feedback SyncDetectedFb { get; set; }
    }
}
