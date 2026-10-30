using System;
using System.Linq;
using Crestron.SimplSharpPro.AudioDistribution;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Extensions;
using NvxEpi.Features.Routing;
using NvxEpi.JoinMaps;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices;

/// <summary>
/// DM-NAX-AUD-IO used as a NAX (AES67) audio receiver: subscribes one of its NAX receive streams to an NVX
/// encoder's secondary audio stream and sends it to its output zone.
/// It is not an NVX endpoint (no video stream, no NVX hardware), so it stays out of the NVX tie lines and
/// matrix routing; route it through dynNvx "naxReceivers" or its own bridge.
/// </summary>
public class DmNaxAudIo : CrestronGenericBridgeableBaseDevice
{
    public const string RouteNameKey = "CurrentNaxAudioRoute";
    public const string RouteValueKey = "CurrentNaxAudioRouteValue";

    private readonly DmNaxAudIoCommercial _hardware;
    private readonly DmNaxAudIoProperties _props;
    private ISecondaryAudioStream _current;

    public StringFeedback RxAudioAddress { get; private set; }
    public StringFeedback StreamStatus { get; private set; }
    public BoolFeedback IsStreaming { get; private set; }

    /// <summary>The encoder whose secondary audio this unit receives (DeviceId), 0 when cleared.</summary>
    public IntFeedback CurrentRouteValue { get; private set; }
    public StringFeedback CurrentRouteName { get; private set; }

    public DmNaxAudIo(DeviceConfig config, DmNaxAudIoProperties props, DmNaxAudIoCommercial hardware)
        : base(config.Key, config.Name, hardware)
    {
        _hardware = hardware;
        _props = props;

        RxAudioAddress = new StringFeedback("NaxRxAddress", () => Rx?.MulticastAdddressFeedback.StringValue ?? string.Empty);
        StreamStatus = new StringFeedback("NaxRxStreamStatus", () => Rx?.StreamStatusFeedback.ToString() ?? string.Empty);
        IsStreaming = new BoolFeedback("NaxRxIsStreaming", () => Rx != null && Rx.StreamStatusFeedback == eStreamStatus.StreamStarted);
        CurrentRouteValue = new IntFeedback(RouteValueKey, () => _current?.DeviceId ?? 0);
        CurrentRouteName = new StringFeedback(RouteNameKey, () => _current?.Name ?? NvxGlobalRouter.NoSourceText);

        AddToFeedbackList(RxAudioAddress, StreamStatus, IsStreaming, CurrentRouteValue, CurrentRouteName);
    }

    private DmNaxXNax.NaxReceive Rx =>
        _hardware.Nax.Rx.Keys.Contains((uint)_props.RxStream) ? _hardware.Nax.Rx[(uint)_props.RxStream] : null;

    protected override bool CustomActivate()
    {
        var result = base.CustomActivate();

        if (Rx == null)
        {
            this.LogError("NAX receive stream {rx} not found; it has {count}", _props.RxStream, _hardware.Nax.Rx.Count);
            return result;
        }

        Rx.NaxTxRxPropertyChange += (sender, args) =>
        {
            RxAudioAddress.FireUpdate();
            StreamStatus.FireUpdate();
            IsStreaming.FireUpdate();
            UpdateCurrentRoute();
        };

        _hardware.OnlineStatusChange += (sender, args) =>
        {
            if (args.DeviceOnLine)
                SetDefaults();
        };

        return result;
    }

    /// <summary>The unit keeps its own NAX settings; these are reapplied each time it comes online.</summary>
    private void SetDefaults()
    {
        Rx?.AutomaticInitiationEnable();

        if (_props.Zone > 0 && _hardware.Routing.Zones.Keys.Contains((uint)_props.Zone))
            _hardware.Routing.Zones[(uint)_props.Zone].UShortValue = (ushort)_props.ZoneSource;
    }

    /// <summary>Receive the encoder's secondary audio stream; null clears it.</summary>
    public void RouteFrom(ISecondaryAudioStream tx)
    {
        if (tx == null || !tx.IsTransmitter)
        {
            Clear();
            return;
        }

        tx.TxAudioAddress.FireUpdate();
        var address = tx.TxAudioAddress.StringValue;

        if (string.IsNullOrEmpty(address) || address == SecondaryAudioExtensions.NoRouteString)
        {
            this.LogWarning("{tx} has no secondary audio address; clearing", tx.Key);
            Clear();
            return;
        }

        this.LogDebug("Receiving {tx} secondary audio at {address}", tx.Key, address);
        SetAddress(address);
    }

    public void Clear()
    {
        this.LogDebug("Clearing NAX audio route");
        SetAddress(SecondaryAudioExtensions.NoRouteString);
    }

    private void SetAddress(string address)
    {
        if (Rx == null)
            return;

        Rx.MulticastAddress.StringValue = address;

        if (address == SecondaryAudioExtensions.NoRouteString)
            Rx.StopStream();
        else
            Rx.StartStream();
    }

    /// <summary>Route by the encoder's NVX DeviceId; 0 clears.</summary>
    public void RouteFrom(int txDeviceId)
    {
        if (txDeviceId == 0)
        {
            Clear();
            return;
        }

        var tx = DeviceManager.AllDevices
            .OfType<ISecondaryAudioStream>()
            .FirstOrDefault(d => d.IsTransmitter && d.DeviceId == txDeviceId);

        if (tx == null)
        {
            this.LogWarning("No NVX transmitter with DeviceId {id}", txDeviceId);
            return;
        }

        RouteFrom(tx);
    }

    private void UpdateCurrentRoute()
    {
        var address = RxAudioAddress.StringValue;

        _current = string.IsNullOrEmpty(address) || address == SecondaryAudioExtensions.NoRouteString
            ? null
            : DeviceManager.AllDevices
                .OfType<ISecondaryAudioStream>()
                .FirstOrDefault(d => d.IsTransmitter && address.Equals(d.TxAudioAddress.StringValue));

        CurrentRouteValue.FireUpdate();
        CurrentRouteName.FireUpdate();
    }

    /// <summary>
    /// Same joins as an NVX receiver for what applies: Online (d1), Stream Started (d2), Audio Route by encoder
    /// DeviceId (a2, set/get), Audio Route name (s2), Device Name (s11), NAX Rx Address (s18).
    /// </summary>
    public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        var joinMap = new NvxDeviceJoinMap(joinStart);

        var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);
        if (customJoins != null)
            joinMap.SetCustomJoinData(customJoins);

        bridge?.AddJoinMap(Key, joinMap);

        IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);
        IsStreaming.LinkInputSig(trilist.BooleanInput[joinMap.StreamStarted.JoinNumber]);
        CurrentRouteValue.LinkInputSig(trilist.UShortInput[joinMap.AudioRoute.JoinNumber]);
        CurrentRouteName.LinkInputSig(trilist.StringInput[joinMap.AudioRouteString.JoinNumber]);
        RxAudioAddress.LinkInputSig(trilist.StringInput[joinMap.NaxRxAddress.JoinNumber]);
        trilist.SetString(joinMap.DeviceName.JoinNumber, Name);

        trilist.SetUShortSigAction(joinMap.AudioRoute.JoinNumber, value => RouteFrom(value));

        trilist.OnlineStatusChange += (sender, args) =>
        {
            if (!args.DeviceOnLine)
                return;

            trilist.SetString(joinMap.DeviceName.JoinNumber, Name);
            foreach (var feedback in Feedbacks)
                feedback.FireUpdate();
        };
    }
}

public class DmNaxAudIoProperties
{
    [JsonProperty("control")]
    public EssentialsControlPropertiesConfig Control { get; set; }

    /// <summary>NAX receive stream that takes the encoder audio (1-based).</summary>
    [JsonProperty("rxStream")]
    public int RxStream { get; set; } = 1;

    /// <summary>Output zone set to <see cref="ZoneSource"/> when the unit comes online; 0 leaves routing to the unit.</summary>
    [JsonProperty("zone")]
    public int Zone { get; set; } = 1;

    /// <summary>Zone source value; 17 = AES67 stream.</summary>
    [JsonProperty("zoneSource")]
    public int ZoneSource { get; set; } = 17;
}
