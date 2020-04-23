using PepperDash.Essentials.Core;
namespace NvxEpi.Interfaces
{
    public interface INvxHdmiInputHelper
    {
        int HdmiCapability { get; set; }
        Feedback HdmiCapabilityFb { get; set; }
        int HdmiSupportedLevel { get; set; }
        Feedback HdmiSupportedLevelFb { get; set; }
        bool SyncDetected { get; }
        Feedback SyncDetectedFb { get; set; }
    }
}
