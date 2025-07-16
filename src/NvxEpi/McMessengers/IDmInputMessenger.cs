using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions.DmInput;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace NvxEpi.McMessengers;

public class IDmInputMessenger : MessengerBase
{
    private readonly IDmInput device;

    private readonly Timer debounceTimer;

    public IDmInputMessenger(string key, string messagePath, IDmInput device) : base(key, messagePath, device)
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

        foreach (var feedback in device.Feedbacks.Where((f) => f.Key.Contains("Dm1")))
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
        var dmInputs = GetInputState();


        var message = new DmInputFullState
        {
            DmInputs = dmInputs,
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
            dmInputs = GetInputState()
        }));
    }

    private Dictionary<string, DmInputState> GetInputState()
    {
        var dmInputs = device.Hardware.DmIn == null
            ? new Dictionary<string, DmInputState>()
            : new Dictionary<string, DmInputState>
            {
                { "1", new DmInputState(device, 1) }
            };

        return dmInputs;
    }
}

public class DmInputFullState: DeviceStateMessageBase
{
    [JsonProperty("dmInputs")]
    public Dictionary<string, DmInputState> DmInputs { get; set; }
}

public class DmInputState
{
    private readonly IDmInput device;
    private readonly uint input;
    [JsonIgnore]
    public string Key => $"DmIn{input}";

    [JsonProperty("syncDetected")]
    public bool SyncDetected => device.SyncDetected[input].BoolValue;

    public DmInputState(IDmInput device, uint input)
    {
        this.device = device;
        this.input = input;
    }
}
