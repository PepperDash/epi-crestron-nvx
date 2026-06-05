using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NvxEpi
{
    internal class NvxNetworkPortInfo : INvxNetworkPortInformation
    {
        private readonly Timer debounceTimer;
        private readonly DmNvxBaseClass device;
        private readonly List<NvxNetworkPortInfo> networkPorts = new();

        public NvxNetworkPortInfo(string key, DmNvxBaseClass device)
        {
            Key = key;
            this.device = device;

            debounceTimer = new Timer(
                _ => PortInformationChanged?.Invoke(this, EventArgs.Empty), 
                null,
                Timeout.Infinite,
                Timeout.Infinite);

            device.Network.NetworkChange += (s, e) => debounceTimer.Change(1000, System.Threading.Timeout.Infinite);
        }

        public List<NvxNetworkPortInformation> NetworkPorts =>
            device
                .Network.LldpPort.Values.Select(port => new NvxNetworkPortInformation(
                    port,
                    port.Number
                ))
            .ToList();

        public string Key { get; }

        public event EventHandler? PortInformationChanged;
    }
}
