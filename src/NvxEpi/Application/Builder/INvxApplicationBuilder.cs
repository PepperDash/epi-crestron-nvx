using System.Collections.Generic;
using NvxEpi.Application.Config;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Builder
{
    public interface INvxApplicationBuilder : IKeyed
    {
        Dictionary<int, NvxApplicationDeviceVideoConfig> Transmitters { get; }
        Dictionary<int, NvxApplicationDeviceVideoConfig> Receivers { get; }
        Dictionary<int, NvxApplicationDeviceAudioConfig> AudioTransmitters { get; }
        Dictionary<int, NvxApplicationDeviceAudioConfig> AudioReceivers { get; }

        EssentialsDevice Build();
    }
}