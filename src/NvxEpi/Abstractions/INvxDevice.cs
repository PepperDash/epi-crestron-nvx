using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions
{
    public interface INvxDevice : IRoutingInputsOutputs, INvxHardware, IMulticastAddress, 
        IHasFeedback, IOnline
    {
          
    }
}