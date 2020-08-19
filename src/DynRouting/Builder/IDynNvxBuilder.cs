using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.DynRouting.Builder
{
    public interface IDynNvxBuilder : IKeyed
    {
        Dictionary<int, string> Transmitters { get; }
        Dictionary<int, string> Receivers { get; }

        EssentialsDevice Build();
    }
}