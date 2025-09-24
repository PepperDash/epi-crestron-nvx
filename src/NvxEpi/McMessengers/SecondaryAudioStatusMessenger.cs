using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers;

public class SecondaryAudioStatusMessenger : MessengerBase
{
    private readonly NvxBaseDevice device;

    private readonly Timer debounceTimer;
    public SecondaryAudioStatusMessenger(string key, string path, NvxBaseDevice device) : base(key, path, device)
    {
        this.device = device;

        debounceTimer = new Timer(1000)
        {
            Enabled = false,
            AutoReset = false,
        };

        debounceTimer.Elapsed += (o, a) => SendUpdate(o, null);
    }

    protected override void RegisterActions()
    {
        base.RegisterActions();

        AddAction("/fullStatus", SendFullStatus);
        AddAction("/secondaryAudioStatus", SendFullStatus);

        device.IsStreamingSecondaryAudio.OutputChange += Debounce;
        device.SecondaryAudioStreamStatus.OutputChange += Debounce;
        device.SecondaryAudioAddress.OutputChange += Debounce;
    }

    private void Debounce(object sender, FeedbackEventArgs e)
    {
        debounceTimer.Stop();
        debounceTimer.Start();
    }

    private void SendFullStatus(string id, JToken content)
    {
        PostStatusMessage(new SecondaryAudioStateMessage(device), id);
    }

    private void SendUpdate(object sender, FeedbackEventArgs args)
    {
        PostStatusMessage(JToken.FromObject(new SecondaryAudioUpdateMessage(device)));
    }
}

public class SecondaryAudioStateMessage : DeviceStateMessageBase
{
    [JsonIgnore]
    private readonly NvxBaseDevice device;

    [JsonProperty("isStreamingSecondaryAudio")]
    public bool IsStreamingSecondaryAudio => device.IsStreamingSecondaryAudio.BoolValue;

    [JsonProperty("secondaryAudioStreamStatus")]
    public string SecondaryAudioStreamStatus => device.SecondaryAudioStreamStatus.StringValue;

    [JsonProperty("secondaryAudioStreamUrl")]
    public string SecondaryAudioStreamUrl => device.SecondaryAudioAddress.StringValue;

    public SecondaryAudioStateMessage(NvxBaseDevice device)
    {
        this.device = device;
    }
}

public class SecondaryAudioUpdateMessage : DeviceStateMessageBase
{
    [JsonIgnore]
    private readonly NvxBaseDevice device;

    [JsonProperty("isStreamingSecondaryAudio")]
    public bool IsStreamingSecondaryAudio => device.IsStreamingSecondaryAudio.BoolValue;

    [JsonProperty("secondaryAudioStreamStatus")]
    public string SecondaryAudioStreamStatus => device.SecondaryAudioStreamStatus.StringValue;

    [JsonProperty("secondaryAudioStreamUrl")]
    public string SecondaryAudioStreamUrl => device.SecondaryAudioAddress.StringValue;

    public SecondaryAudioUpdateMessage(NvxBaseDevice device)
    {
        this.device = device;
    }
}
