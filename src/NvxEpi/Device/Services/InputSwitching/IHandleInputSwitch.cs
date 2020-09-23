using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.InputSwitching
{
    public interface IHandleInputSwitch
    {
        void HandleSwitch(object input, eRoutingSignalType type);
    }
}