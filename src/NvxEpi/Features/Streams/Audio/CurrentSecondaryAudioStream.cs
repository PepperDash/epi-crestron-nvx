using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Features.Routing;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Audio;

public class CurrentSecondaryAudioStream : SecondaryAudioStream, ICurrentSecondaryAudioStream
{
    public const string RouteNameKey = "CurrentSecondaryAudioRoute";
    public const string RouteValueKey = "CurrentSecondaryAudioRouteValue";

    private readonly IntFeedback _currentSecondaryAudioStreamId;
    private readonly StringFeedback _currentSecondaryAudioStreamName;

    private readonly IList<ISecondaryAudioStream> _audioTransmitters = new List<ISecondaryAudioStream>();
    private readonly CCriticalSection _lock = new();

    private ISecondaryAudioStream _current;

    public CurrentSecondaryAudioStream(INvxDeviceWithHardware device) : base(device)
    {
        _currentSecondaryAudioStreamId = new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default);
        _currentSecondaryAudioStreamName = new StringFeedback(RouteNameKey, () => _current != null ? _current.Name : NvxGlobalRouter.NoSourceText);

        Feedbacks.Add(CurrentSecondaryAudioStreamId);
        Feedbacks.Add(CurrentSecondaryAudioStreamName);

        Initialize();
    }

    public IntFeedback CurrentSecondaryAudioStreamId
    {
        get { return _currentSecondaryAudioStreamId; }
    }

    public StringFeedback CurrentSecondaryAudioStreamName
    {
        get { return _currentSecondaryAudioStreamName; }
    }

    private ISecondaryAudioStream GetCurrentAudioStream()
    {
        if (string.IsNullOrEmpty(RxAudioAddress.StringValue) || RxAudioAddress.StringValue.Equals("0.0.0.0"))
            return null;

        var result = _audioTransmitters
            .Where(x => !string.IsNullOrEmpty(x.TxAudioAddress.StringValue))
            .FirstOrDefault(
                x => x.TxAudioAddress.StringValue.Equals(RxAudioAddress.StringValue));

        if (result != null)
            return result;

        result = DeviceManager
            .AllDevices
            .OfType<ISecondaryAudioStream>()
            .Where(x => !string.IsNullOrEmpty(x.TxAudioAddress.StringValue))
            .FirstOrDefault(
                tx => tx.TxAudioAddress.StringValue.Equals(RxAudioAddress.StringValue));

        if (result != null)
            _audioTransmitters.Add(result);

        return result;
    }

    private void Initialize()
    {
        IsOnline.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
        IsStreamingSecondaryAudio.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
        SecondaryAudioStreamStatus.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
        RxAudioAddress.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
    }

    private void UpdateCurrentAudioRoute()
    {
        if (!IsOnline.BoolValue)
            return;

        try
        {
            _lock.Enter();
            _current = GetCurrentAudioStream();

            CurrentSecondaryAudioStreamId.FireUpdate();
            CurrentSecondaryAudioStreamName.FireUpdate();
        }
        catch (Exception ex)
        {
            this.LogError("Error getting current audio route : {message}", ex.Message);
            this.LogDebug(ex, "Stack trace: ");
        }
        finally
        {
            _lock.Leave();
        }
    }
}