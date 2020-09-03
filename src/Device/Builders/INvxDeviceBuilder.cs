using System;
using System.Collections.Generic;
using Crestron.SimplSharp.Ssh;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Builders
{
    public interface INvxDeviceBuilder
    {
        int VirtualDeviceId { get; }
        DeviceConfig Config { get; }
        DmNvxBaseClass Device { get; }
        BoolFeedback IsTransmitter { get; }
        INvxDevice Build();
    }
}