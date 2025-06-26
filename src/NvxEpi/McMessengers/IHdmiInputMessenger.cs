using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace NvxEpi.McMessengers;

public class IHdmiInputMessenger : MessengerBase
{
    private readonly IHdmiInput device;

    private readonly Timer debounceTimer;

    public IHdmiInputMessenger(string key, string messagePath, IHdmiInput device) : base(key, messagePath, device)
    {
        this.device = device;

        debounceTimer = new Timer(1000)
        {
            Enabled = false,
            AutoReset = false,
        };

        debounceTimer.Elapsed += (o, a) => UpdateStatus();
    }

    protected override void RegisterActions()
    {
        base.RegisterActions();

        AddAction("/fullStatus",(id, content) => SendFullStatus());

        foreach (var feedback in device.Feedbacks.Where((f) => f.Key.Contains("Hdmi1") || f.Key.Contains("Hdmi2")))
        {
            feedback.OutputChange += (o, a) => {
                // Debounce the status update to avoid flooding the server with messages
                debounceTimer.Stop();
                debounceTimer.Start();
            };
        }
    }

    private void SendFullStatus()
    {
        var hdmiInputs = GetInputState();


        var message = new HdmiInputFullState
        {
            HdmiInputs = hdmiInputs,
        };

        try
        {
            PostStatusMessage(message);
        } catch(Exception e)
        {
            this.LogError(e, "Exception sending message {exception}");
        }
    }

    private void UpdateStatus()
    {           
        PostStatusMessage(JToken.FromObject(new {
            hdmiInputs = GetInputState()
        }));
    }

    private Dictionary<string, HdmiInputState> GetInputState()
    {            
        var hdmiInputs = device.Hardware.HdmiIn == null
            ? new Dictionary<string, HdmiInputState>()
            : device.Hardware.HdmiIn.Keys.Select(hdmiIn =>
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Getting Hdmi Input State for {input}",this, hdmiIn);
                return new HdmiInputState(device, hdmiIn);
            }).ToDictionary(hdmiInState => hdmiInState.Key, hdmiInputState => hdmiInputState);

        return hdmiInputs;
    }
}

public class HdmiInputFullState: DeviceStateMessageBase
{
    [JsonProperty("hdmiInputs")]
    public Dictionary<string, HdmiInputState> HdmiInputs { get; set; }
}

public class HdmiInputState
{
    private readonly IHdmiInput device;
    private readonly uint input;
    [JsonIgnore]
    public string Key => $"HdmiIn{input}";

    [JsonProperty("hdcpCapability")]        
    public string HdcpCapability => device.HdcpCapabilityString[input].StringValue;

    [JsonProperty("hdcpSupport")]
    public string HdcpSupport => device.HdcpSupport[input].StringValue;

    [JsonProperty("syncDetected")]
    public bool SyncDetected => device.SyncDetected[input].BoolValue;

    [JsonProperty("currentResolution")]
    public string CurrentResolution => device.CurrentResolution[input].StringValue;

    [JsonProperty("audioChannelCount")]
    public int AudioChannelCount => device.AudioChannels[input].IntValue;

    [JsonProperty("audioFormat")]
    public string AudioFormat => device.AudioFormat[input].StringValue;

    [JsonProperty("colorspaceMode")]
    public string ColorspaceMode => device.ColorSpace[input].StringValue;

    [JsonProperty("hdrType")]
    public string HdrType => device.HdrType[input].StringValue;

    public HdmiInputState(IHdmiInput device, uint input)
    {
        this.device = device;
        this.input = input;
    }
}
