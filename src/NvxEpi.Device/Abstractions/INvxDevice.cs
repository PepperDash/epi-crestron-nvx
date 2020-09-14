using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Abstractions
{
    public interface INvxDevice : IHardware, IKeyName, IHasFeedback, IRoutingInputsOutputs
    {
        int VirtualDeviceId { get; }
        DeviceConfig Config { get; }
        BoolFeedback IsTransmitter { get; }    
        StringFeedback DeviceName { get; }
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
        StringFeedback StreamUrl { get; }
        StringFeedback MulticastAddress { get; }
    }
}