using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using NvxEpi.Device.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IRoutingCommand : IKeyed
    {
        void HandleRoute(RoutingInputPortEnum input, eRoutingSignalType type);
    }
}