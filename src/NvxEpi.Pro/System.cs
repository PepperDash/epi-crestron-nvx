using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials;

namespace NvxEpi.Pro
{
    public class System : ControlSystem
    {
        public override void InitializeSystem()
        {
            Debug.Console(0, "HELLO WORLD");
            base.InitializeSystem();
        }
    }
}