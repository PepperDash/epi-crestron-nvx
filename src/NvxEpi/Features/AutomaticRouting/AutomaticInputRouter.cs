using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Extensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.AutomaticRouting
{
    public class AutomaticInputRouter
    {
        private readonly IHdmiInput _currentVideoInput;
        private readonly BoolFeedbackOr _hdmiInputDetected = new BoolFeedbackOr();

        public AutomaticInputRouter(IHdmiInput currentVideoInput)
        {
            _currentVideoInput = currentVideoInput;
            foreach (var fb in currentVideoInput.SyncDetected.Values)
                _hdmiInputDetected.AddOutputIn(fb);

            _hdmiInputDetected.Output.OutputChange += OnSyncDetected;

            var stream = _currentVideoInput as IStream;
            if (stream == null || _currentVideoInput.IsTransmitter)
                return;

            stream.IsStreamingVideo.OutputChange += OnSyncDetected;
        }

        private void OnSyncDetected(object sender, FeedbackEventArgs feedbackEventArgs)
        {
            var currentVidoInput = _currentVideoInput as ICurrentVideoInput;
            if (currentVidoInput == null)
                return;

            BoolFeedback hdmi1;
            if (_currentVideoInput.SyncDetected.TryGetValue(1, out hdmi1))
            {
                currentVidoInput.SetVideoToHdmiInput1();
                return;
            }

            BoolFeedback hdmi2;
            if (_currentVideoInput.SyncDetected.TryGetValue(2, out hdmi2))
            {
                currentVidoInput.SetVideoToHdmiInput2();
                return;
            }

            if (!(_currentVideoInput is IStream) || _currentVideoInput.IsTransmitter)
                return;

            currentVidoInput.SetVideoToStream();
        }
    }
}