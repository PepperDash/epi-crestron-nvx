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

        private readonly IList<ISecondaryAudioStream> _audioTransmitters = new List<ISecondaryAudioStream>(); 
        private readonly CCriticalSection _lock = new CCriticalSection();

        private ISecondaryAudioStream _current;

        public CurrentSecondaryAudioStream(INvxDeviceWithHardware device) : base(device)
        {
            _currentSecondaryAudioStreamId = new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default( int ));

            _currentSecondaryAudioStreamName = new StringFeedback(RouteNameKey, () => _current != null ? _current.AudioSourceName.StringValue : NvxGlobalRouter.NoSourceText);

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

            var result = _audioTransmitters
                .FirstOrDefault(
                    x => x.TxAudioAddress.StringValue.Equals(RxAudioAddress.StringValue));

            if (result != null)
                return result;

            result = DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .FirstOrDefault(
                    tx => tx.TxAudioAddress.StringValue.Equals(TxAudioAddress.StringValue));

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