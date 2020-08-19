using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Essentials.Bridges;
using DmChassisControllerJoinMap = PepperDash.Essentials.Core.Bridges.DmChassisControllerJoinMap;

namespace NvxEpi.DynRouting.JoinMap
{
    public class DynRoutingJoinMap : DmChassisControllerJoinMap
    {
        protected DynRoutingJoinMap(uint joinStart, Type type) : base(joinStart, type)
        {
        }
    }
}