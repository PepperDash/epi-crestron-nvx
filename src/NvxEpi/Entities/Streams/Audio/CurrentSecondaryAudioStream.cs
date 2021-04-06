using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Audio
{
    public class CurrentSecondaryAudioStream : SecondaryAudioStream, ICurrentSecondaryAudioStream
    {
        public const string RouteNameKey = "CurrentSecondaryAudioRoute";
        public const string RouteValueKey = "CurrentSecondaryAudioRouteValue";

        private readonly IntFeedback _currentSecondaryAudioStreamId;
        private readonly StringFeedback _currentSecondaryAudioStreamName;

        private readonly IList<ISecondaryAudioStream> _transmitters = new List<ISecondaryAudioStream>(); 
        private readonly CCriticalSection _lock = new CCriticalSection();

        private ISecondaryAudioStream _current;

        public CurrentSecondaryAudioStream(INvxDeviceWithHardware device) : base(device)
        {
            _currentSecondaryAudioStreamId = IsTransmitter
                ? new IntFeedback(() => default( int ))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default( int ));

            _currentSecondaryAudioStreamName = IsTransmitter
                ? new StringFeedback(() => String.Empty)
                : new StringFeedback(RouteNameKey,
                    () => _current != null ? _current.AudioName.StringValue : NvxGlobalRouter.NoSourceText);

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
            if (!IsStreamingSecondaryAudio.BoolValue)
                return null;

            var result = _transmitters
                .FirstOrDefault(
                    x => x.SecondaryAudioAddress.StringValue.Equals(SecondaryAudioAddress.StringValue));

            if (result != null)
                return result;

            result = DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(t => t.IsTransmitter)
                .FirstOrDefault(
                    tx => tx.SecondaryAudioAddress.StringValue.Equals(SecondaryAudioAddress.StringValue));

            if (result != null)
                _transmitters.Add(result);

            return result;      
        }

        private void Initialize()
        {
            IsOnline.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            IsStreamingSecondaryAudio.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioStreamStatus.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioAddress.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
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
            catch (Exception ex)
            {
                Debug.Console(1,
                    this,
                    "Error getting current audio route : {0}\r{1}\r{2}",
                    ex.Message,
                    ex.InnerException,
                    ex.StackTrace);
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}