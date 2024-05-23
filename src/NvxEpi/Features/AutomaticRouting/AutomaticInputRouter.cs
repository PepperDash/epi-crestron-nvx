using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Extensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.AutomaticRouting;

public class AutomaticInputRouter
{
    private readonly IHdmiInput _currentVideoInput;
    private readonly BoolFeedbackOr _hdmiInputDetected = new();

    public AutomaticInputRouter(IHdmiInput currentVideoInput)
    {
        _currentVideoInput = currentVideoInput;
        foreach (var fb in currentVideoInput.SyncDetected.Values)
            _hdmiInputDetected.AddOutputIn(fb);

        _hdmiInputDetected.Output.OutputChange += OnSyncDetected;

        if (_currentVideoInput is not IStream stream || _currentVideoInput.IsTransmitter)
            return;

        stream.IsStreamingVideo.OutputChange += OnSyncDetected;
    }

    private void OnSyncDetected(object sender, FeedbackEventArgs feedbackEventArgs)
    {
        if (_currentVideoInput is not ICurrentVideoInput currentVidoInput)
            return;
        if (_currentVideoInput.SyncDetected.TryGetValue(1, out _))
        {
            currentVidoInput.SetVideoToHdmiInput1();
            return;
        }

        if (_currentVideoInput.SyncDetected.TryGetValue(2, out _))
        {
            currentVidoInput.SetVideoToHdmiInput2();
            return;
        }

        if (_currentVideoInput is not IStream || _currentVideoInput.IsTransmitter)
            return;

        currentVidoInput.SetVideoToStream();
    }
}