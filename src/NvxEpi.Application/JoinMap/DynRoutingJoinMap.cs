using System;
using DmChassisControllerJoinMap = PepperDash.Essentials.Core.Bridges.DmChassisControllerJoinMap;

namespace NvxEpi.Application.JoinMap
{
    public class DynRoutingJoinMap : DmChassisControllerJoinMap
    {
        protected DynRoutingJoinMap(uint joinStart, Type type) : base(joinStart, type)
        {
        }
    }
}