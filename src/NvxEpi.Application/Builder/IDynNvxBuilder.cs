using System.Collections.Generic;
using PepperDash.Core;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Builder
{
    public interface IDynNvxBuilder : IKeyed
    {
        Dictionary<int, string> Transmitters { get; }
        Dictionary<int, string> Receivers { get; }
        Dictionary<int, MockDisplay> VideoDestinations { get; }
        Dictionary<int, Amplifier> AudioDestinations { get; }
        Dictionary<int, DummyRoutingInputsDevice> Sources { get; }

        EssentialsDevice Build();
    }
}