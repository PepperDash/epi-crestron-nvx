﻿using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Builder
{
    public interface INvxApplicationBuilder : IKeyed
    {
        Dictionary<int, string> Transmitters { get; }
        Dictionary<int, string> Receivers { get; }

        EssentialsDevice Build();
    }
}