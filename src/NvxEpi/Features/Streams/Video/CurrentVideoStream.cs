using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Features.Routing;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Video;

public class CurrentVideoStream : VideoStream, ICurrentStream
{
    public const string RouteNameKey = "CurrentVideoRoute";
    public const string RouteValueKey = "CurrentVideoRouteValue";
    private readonly IntFeedback _currentStreamId;
    private readonly StringFeedback _currentStreamName;
    private readonly CCriticalSection _lock = new();
    private IStream _current;

    private readonly IList<IStream> _transmitters = new List<IStream>();

    public CurrentVideoStream(INvxDeviceWithHardware device) : base(device)
    {
        _currentStreamId = IsTransmitter
            ? new IntFeedback("currentStreamId", () => default)
            : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default);

        _currentStreamName = IsTransmitter
            ? new StringFeedback("currentStreamName", () => string.Empty)
            : new StringFeedback(RouteNameKey,
                () => _current != null ? _current.Name : NvxGlobalRouter.NoSourceText);

        Feedbacks.Add(_currentStreamId);
        Feedbacks.Add(_currentStreamName);

        Initialize();
    }

    public IntFeedback CurrentStreamId
    {
        get { return _currentStreamId; }
    }

    public StringFeedback CurrentStreamName
    {
        get { return _currentStreamName; }
    }

    public void UpdateCurrentRoute()
    {
        if (!IsOnline.BoolValue || IsTransmitter)
            return;

        try
        {
            _lock.Enter();
            _current = GetCurrentStream();



            this.LogVerbose("Current stream address: {address} device ID: {id}", _current?.MulticastAddress.StringValue ?? "0.0.0.0", _current?.DeviceId ?? 0);


            CurrentStreamId.FireUpdate();
            CurrentStreamName.FireUpdate();
        }
        catch (Exception ex)
        {
            this.LogError("Error getting current video route : {message}", ex.Message);
            this.LogDebug(ex, "Stack trace: ");
        }
        finally
        {
            _lock.Leave();
        }
    }

    private IStream GetCurrentStream()
    {
        if (string.IsNullOrEmpty(StreamUrl.StringValue) || MulticastAddress.StringValue.Equals("0.0.0.0"))
            return null;

        var result = _transmitters
            .Where(x => !string.IsNullOrEmpty(x.StreamUrl.StringValue))
            .FirstOrDefault(
                x => x.StreamUrl.StringValue.Equals(StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));

        if (result != null)
        {
            return result;
        }

        result = DeviceManager
            .AllDevices
            .OfType<IStream>()
            .Where(t => t.IsTransmitter)
            .Where(x => !string.IsNullOrEmpty(x.StreamUrl.StringValue))
            .FirstOrDefault(
                tx => tx.StreamUrl.StringValue.Equals(StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));

        if (result != null)
        {
            _transmitters.Add(result);
        }

        return result;
    }

    private void Initialize()
    {
        IsOnline.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        VideoStreamStatus.OutputChange += (sender, args) => UpdateCurrentRoute();
        IsStreamingVideo.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        MulticastAddress.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        StreamUrl.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
    }
}