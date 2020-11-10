using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Entities.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Audio
{
    public class CurrentSecondaryAudioStream : SecondaryAudioStream, ICurrentSecondaryAudioStream
    {
        public const string RouteNameKey = "CurrentSecondaryAudioRoute";
        public const string RouteValueKey = "CurrentSecondaryAudioRouteValue";
        private readonly CCriticalSection _lock = new CCriticalSection();
        private ISecondaryAudioStream _current;

        public CurrentSecondaryAudioStream(INvxHardware device) : base(device)
        {
            Initialize();
        }

        public IntFeedback CurrentSecondaryAudioStreamId { get; private set; }
        public StringFeedback CurrentSecondaryAudioStreamName { get; private set; }

        private ISecondaryAudioStream GetCurrentAudioStream()
        {
            try
            {
                if (!IsStreamingSecondaryAudio.BoolValue)
                    return null;

                if (Hardware.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return null;

                if (Hardware.Control.ActiveAudioSourceFeedback != DmNvxControl.eAudioSource.SecondaryStreamAudio)
                    return null;

                return DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStream>()
                    .Where(t => t.IsTransmitter && t.IsStreamingSecondaryAudio.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.SecondaryAudioAddress.StringValue.Equals(
                                SecondaryAudioAddress.StringValue,
                                StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Debug.Console(1,
                    this,
                    "Error getting current audio route : {0}\r{1}\r{2}",
                    ex.Message,
                    ex.InnerException,
                    ex.StackTrace);
                throw;
            }
        }

        private void Initialize()
        {
            CurrentSecondaryAudioStreamId = IsTransmitter
                ? new IntFeedback(RouteValueKey, () => default( int ))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default( int ));

            CurrentSecondaryAudioStreamName = IsTransmitter
                ? new StringFeedback(RouteNameKey, () => String.Empty)
                : new StringFeedback(RouteNameKey,
                    () => _current != null ? _current.AudioName.StringValue : NvxGlobalRouter.NoSourceText);

            IsOnline.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            IsStreamingSecondaryAudio.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioStreamStatus.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioAddress.OutputChange += (sender, args) => UpdateCurrentAudioRoute();

            Feedbacks.Add(CurrentSecondaryAudioStreamId);
            Feedbacks.Add(CurrentSecondaryAudioStreamName);
        }

        private void UpdateCurrentAudioRoute()
        {
            if (!IsOnline.BoolValue || IsTransmitter)
                return;

            try
            {
                _lock.Enter();
                _current = GetCurrentAudioStream();

                CurrentSecondaryAudioStreamId.FireUpdate();
                CurrentSecondaryAudioStreamName.FireUpdate();
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}