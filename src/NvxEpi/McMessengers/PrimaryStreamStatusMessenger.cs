using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers;

public class PrimaryStreamStatusMessenger : MessengerBase
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
        AddAction("/videoStreamStatus", SendFullStatus);

        device.IsStreamingVideo.OutputChange += Debounce;
        device.VideoStreamStatus.OutputChange += Debounce;
        device.StreamUrl.OutputChange += Debounce;
        device.MulticastAddress.OutputChange += Debounce;

        // The technician NVX pages also show these (see StreamFields).
        StreamFields.Subscribe(device, Debounce);
    }

    private void Debounce(object sender, FeedbackEventArgs args)
    {
        // Debounce the status update to avoid flooding the server with messages
        debounceTimer.Stop();
        debounceTimer.Start();
    }

    private void SendFullStatus(string id, JToken content)
    {
        PostStatusMessage(new StreamStateMessage(device), id);
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

    // What the technician NVX Encoders and Decoders pages show besides the stream URL (a null is a unit without that feature).
    [JsonProperty("multicastVideoAddress")]
    public string MulticastVideoAddress => device.MulticastAddress.StringValue;

    // The secondary audio address is the NAX address (the transmitter's, or the receiver's): there is one audio multicast address.
    [JsonProperty("multicastAudioAddress")]
    public string MulticastAudioAddress => StreamFields.Read(device.SecondaryAudioAddress);

    [JsonProperty("streamActive")]
    public bool StreamActive => device.IsStreamingVideo.BoolValue;

    [JsonProperty("videoInput")]
    public string VideoInput => StreamFields.Read(device.CurrentVideoInput);

    [JsonProperty("audioInput")]
    public string AudioInput => StreamFields.Read(device.CurrentAudioInput);

    [JsonProperty("naxInput")]
    public string NaxInput => StreamFields.Read(device.CurrentNaxInput);

    [JsonProperty("videoStream")]
    public string VideoStream => StreamFields.Read(device.CurrentStreamName);

    [JsonProperty("audioStream")]
    public string AudioStream => StreamFields.Read(device.CurrentSecondaryAudioStreamName);

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

    // What the technician NVX Encoders and Decoders pages show besides the stream URL (a null is a unit without that feature).
    [JsonProperty("multicastVideoAddress")]
    public string MulticastVideoAddress => device.MulticastAddress.StringValue;

    // The secondary audio address is the NAX address (the transmitter's, or the receiver's): there is one audio multicast address.
    [JsonProperty("multicastAudioAddress")]
    public string MulticastAudioAddress => StreamFields.Read(device.SecondaryAudioAddress);

    [JsonProperty("streamActive")]
    public bool StreamActive => device.IsStreamingVideo.BoolValue;

    [JsonProperty("videoInput")]
    public string VideoInput => StreamFields.Read(device.CurrentVideoInput);

    [JsonProperty("audioInput")]
    public string AudioInput => StreamFields.Read(device.CurrentAudioInput);

    [JsonProperty("naxInput")]
    public string NaxInput => StreamFields.Read(device.CurrentNaxInput);

    [JsonProperty("videoStream")]
    public string VideoStream => StreamFields.Read(device.CurrentStreamName);

    [JsonProperty("audioStream")]
    public string AudioStream => StreamFields.Read(device.CurrentSecondaryAudioStreamName);

    public StreamUpdateMessage(NvxBaseDevice device)
    {
        this.device = device;
    }
}

/// <summary>
/// Reads the feedbacks the technician NVX pages show. A unit that does not have a feedback (no NAX, no secondary audio) gives null
/// instead of failing the whole status message.
/// </summary>
internal static class StreamFields
{
    internal static string Read(PepperDash.Essentials.Core.StringFeedback feedback)
    {
        try
        {
            return feedback?.StringValue;
        }
        catch
        {
            return null;
        }
    }

    internal static void Subscribe(NvxBaseDevice device, System.EventHandler<FeedbackEventArgs> handler)
    {
        foreach (var feedback in new PepperDash.Essentials.Core.Feedback[]
                 {
                     device.SecondaryAudioAddress,
                     device.CurrentVideoInput,
                     device.CurrentAudioInput,
                     device.CurrentNaxInput,
                     device.CurrentStreamName,
                     device.CurrentSecondaryAudioStreamName,
                 })
        {
            if (feedback != null) feedback.OutputChange += handler;
        }
    }
}
