using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;
using System.Timers;

namespace NvxEpi.McMessengers;

public class PrimaryStreamStatusMessenger:MessengerBase
{
    private readonly NvxBaseDevice device;

    private readonly Timer debounceTimer;
    public PrimaryStreamStatusMessenger(string key, string messagePath, NvxBaseDevice device) : base(key, messagePath, device)
    {
        this.device = device;

        debounceTimer = new Timer(1000)
        {
            Enabled = false,
            AutoReset = false,
        };

        debounceTimer.Elapsed += (o, a) => HandleUpdate(o, null);
    }

    protected override void RegisterActions()
    {
        base.RegisterActions();

        AddAction("/fullStatus", SendFullStatus);

        device.IsStreamingVideo.OutputChange += Debounce;
        device.VideoStreamStatus.OutputChange += Debounce;
        device.StreamUrl.OutputChange += Debounce;
        device.MulticastAddress.OutputChange += Debounce;
    }

    private void Debounce(object sender, FeedbackEventArgs args)
    {
        // Debounce the status update to avoid flooding the server with messages
        debounceTimer.Stop();
        debounceTimer.Start();
    }

    private void SendFullStatus(string id, JToken content) {
        PostStatusMessage(new StreamStateMessage(device));
    }

    private void HandleUpdate(object sender, FeedbackEventArgs args)
    {
        PostStatusMessage(JToken.FromObject(new StreamUpdateMessage(device)));
    }
}

public class StreamStateMessage : DeviceStateMessageBase
{
    [JsonIgnore]
    private readonly NvxBaseDevice device;

    [JsonProperty("isStreamingVideo")]
    public bool IsStreamingVideo => device.IsStreamingVideo.BoolValue;

    [JsonProperty("videoStreamStatus")]
    public string VideoStreamStatus => device.VideoStreamStatus.StringValue;

    [JsonProperty("streamUrl")]
    public string StreamUrl => device.StreamUrl.StringValue;

    [JsonProperty("multicastAddress")]
    public string MulticastAddress => device.MulticastAddress.StringValue;

    [JsonProperty("isTransmitter")]
    public bool IsTransmitter => device.IsTransmitter;

    public StreamStateMessage(NvxBaseDevice device)
    {
        this.device = device;
    }
}

public class StreamUpdateMessage
{
    [JsonIgnore]
    private readonly NvxBaseDevice device;

    [JsonProperty("isStreamingVideo")]
    public bool IsStreamingVideo => device.IsStreamingVideo.BoolValue;

    [JsonProperty("videoStreamStatus")]
    public string VideoStreamStatus => device.VideoStreamStatus.StringValue;

    [JsonProperty("streamUrl")]
    public string StreamUrl => device.StreamUrl.StringValue;

    [JsonProperty("multicastAddress")]
    public string MulticastAddress => device.MulticastAddress.StringValue;

    [JsonProperty("isTransmitter")]
    public bool IsTransmitter => device.IsTransmitter;

    public StreamUpdateMessage(NvxBaseDevice device)
    {
        this.device = device;
    }
}
