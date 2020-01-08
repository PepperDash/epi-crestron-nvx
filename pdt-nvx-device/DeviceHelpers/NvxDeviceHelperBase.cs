using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PepperDash.Core;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using EssentialsExtensions;
using EssentialsExtensions.Attributes;
using NvxEpi.Interfaces;

namespace NvxEpi.DeviceHelpers
{
    public abstract class NvxDeviceHelperBase : IDynamicFeedback
    {
        protected DmNvxBaseClass _device;

        protected bool _isTransmitter { get { return _device.Control.DeviceModeFeedback == eDeviceMode.Transmitter; } }
        protected bool _isReceiver { get { return !_isTransmitter; } }

        public abstract string Key { get; }

        public NvxDeviceHelperBase(DmNvxBaseClass device)
        {
            _device = device;
        }
    }
}