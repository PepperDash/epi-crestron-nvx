using Crestron.SimplSharpPro;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Device
{
    public interface INvxDevice : IRoutingInputsOutputs, INvxHardware, IMulticastAddress, IHasFeedback, IOnline
    {
          
    }
}