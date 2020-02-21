using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using NvxEpi.Interfaces;

namespace NvxEpi.IRoutingImplementation
{
    public class NvxInputSourceSelector
    {
        public INvxDevice Device { get; set; }
        public int HdmiInput { get; set; }
    }
}