using System.Collections.Generic;

using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace PepperDash.Essentials;

public class Amplifier : EssentialsDevice, IRoutingInputs
{
    public RoutingInputPort CurrentInputPort => AudioIn;

    public RoutingInputPort AudioIn { get; private set; }

    public Amplifier(string key, string name)
        : base(key, name)
    {
        AudioIn = new RoutingInputPort(RoutingPortNames.AnyAudioIn, eRoutingSignalType.Audio,
            eRoutingPortConnectionType.None, null, this);
        InputPorts = new RoutingPortCollection<RoutingInputPort> { AudioIn };
    }

    #region IRoutingInputs Members

    public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }

    #endregion
}

public class AmplifierFactory : EssentialsDeviceFactory<Amplifier>
{
    public AmplifierFactory()
    {
        TypeNames = new List<string>() { "amplifier" };
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        Debug.LogDebug("Factory Attempting to create new Amplifier Device");
        return new Amplifier(dc.Key, dc.Name);
    }
}