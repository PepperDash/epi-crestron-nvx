using System;
using System.Collections.Generic;
using Crestron.SimplSharp.Ssh;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Builders
{
    public interface INvxDeviceBuilder : IKeyName
    {
        DeviceConfig Config { get; }
        DmNvxBaseClass Device { get; }
        Dictionary<NvxDevice.BoolActions, Action<bool>> BoolActions { get; }
        Dictionary<NvxDevice.IntActions, Action<ushort>> IntActions { get; }
        Dictionary<NvxDevice.StringActions, Action<string>> StringActions { get; }
        Dictionary<NvxDevice.DeviceFeedbacks, Feedback> Feedbacks { get; }
        bool IsTransmitter { get; }
        NvxDevice Build();
    }
}