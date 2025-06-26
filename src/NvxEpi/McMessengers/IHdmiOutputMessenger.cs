using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions.HdmiOutput;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace NvxEpi.McMessengers;

public class IHdmiOutputMessenger : MessengerBase
{
    private readonly IHdmiOutput device;

    private readonly Timer debounceTimer;

    public IHdmiOutputMessenger(string key, string messagePath, IHdmiOutput device) : base(key, messagePath, device)
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

        foreach (var feedback in device.Feedbacks.Where((f) => f.Key.Contains("HdmiOut")))
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "feedback key: {key}", this, feedback.Key);
            feedback.OutputChange += (o, a) => {
                // Debounce the status update to avoid flooding the server with messages
                debounceTimer.Stop();
                debounceTimer.Start();
            };
        }
    }

    private void SendFullStatus()
    {        
        var message = new HdmiOutputFullState
        {
            HdmiOutputs = new Dictionary<string, HdmiOutputState>
            {
                {"HdmiOut",new  HdmiOutputState(device) }
            }
        };            

        try
        {

            PostStatusMessage(message);
        } catch(Exception e)
        {
            Debug.LogMessage(e, "Exception sending message {exception}", this, e);
        }
    }

    private void UpdateStatus()
    {           
        PostStatusMessage(JToken.FromObject(new {
            hdmiOutput = new Dictionary<string, HdmiOutputState>
            {
                {"HdmiOut",new  HdmiOutputState(device) }
            }
        }));
    }        
}

public class HdmiOutputFullState: DeviceStateMessageBase
{
    [JsonProperty("hdmiOutputs")]
    public Dictionary<string, HdmiOutputState> HdmiOutputs { get; set; }
}

public class HdmiOutputState
{
    private readonly IHdmiOutput device;

    [JsonProperty("disabledByHdcp")]
    public bool DisabledByHdcp => device.DisabledByHdcp.BoolValue;

    [JsonProperty("outputResolution")]
    public string OutputResolution => device.OutputResolution.StringValue;

    [JsonProperty("edidManufacturer")]
    public string EdidManufacturer => device.EdidManufacturer.StringValue;

    public HdmiOutputState(IHdmiOutput device)
    {
        this.device = device;
    }
}
