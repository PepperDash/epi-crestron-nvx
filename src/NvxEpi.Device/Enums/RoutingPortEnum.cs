using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace NvxEpi.Device.Enums
{
    public class RoutingInputPortEnum : Enumeration<RoutingInputPortEnum>
    {
        private RoutingInputPortEnum(int value, string name) : base(value, name)
        {
            
        }

        public static readonly RoutingInputPortEnum Stream = new RoutingInputPortEnum(0, "Stream");
        public static readonly RoutingInputPortEnum SecondaryAudio = new RoutingInputPortEnum(1, "SecondaryAudio");
        public static readonly RoutingInputPortEnum DmNaxAudio = new RoutingInputPortEnum(2, "DmNax");
    }
}