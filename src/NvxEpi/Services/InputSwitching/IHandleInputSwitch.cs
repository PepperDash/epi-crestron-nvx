using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputSwitching
{
    public interface IHandleInputSwitch
    {
        void HandleSwitch(object input, eRoutingSignalType type);
    }
}