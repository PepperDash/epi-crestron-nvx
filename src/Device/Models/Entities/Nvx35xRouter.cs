using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Entities
{
    public class Nvx35xRouter : IVideoInputSwitcher, IAudioInputSwitcher, IRouting
    {
        private readonly IVideoInputSwitcher _videoInput;
        private readonly IAudioInputSwitcher _audioInput;

        public Nvx35xRouter(IVideoInputSwitcher videoInput, IAudioInputSwitcher audioInput)
        {
            _videoInput = videoInput;
            _audioInput = audioInput;

            BuildRoutingPorts();
        }

        private void BuildRoutingPorts()
        {
            AddStandardPorts();

            if (IsTransmitter.BoolValue)
            {
                InputPorts.Add(
                    new RoutingInputPort(
                        AudioInputEnum.AnalogAudio.Name,
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.LineAudio,
                        new Action(() =>
                        {
                            Debug.Console(1, this, "Making an awesome route : '{0}'", Key);
                            _audioInput.SetInput(AudioInputEnum.AnalogAudio);
                        }),
                        this
                        ));

                OutputPorts.Add(
                    new RoutingOutputPort(VideoOutputEnum.Stream.Name,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        null,
                        this
                        ));
            }
            else
            {
                InputPorts.Add(
                    new RoutingInputPort(
                        VideoInputEnum.Stream.Name,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        new Action(() =>
                        {
                            Debug.Console(1, this, "Making an awesome route : '{0}'", Key);
                            _videoInput.SetInput(VideoInputEnum.Stream);
                            _audioInput.SetInput(AudioInputEnum.AudioFollowsVideo);
                        }),
                        this
                        ));

                OutputPorts.Add(
                    new RoutingOutputPort(
                        AudioOutputEnum.Analog.Name,
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        null,
                        this
                        ));
            }
        }

        private void AddStandardPorts()
        {
            InputPorts.AddRange(new[]
            {
                new RoutingInputPort(
                    VideoInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    new Action(() =>
                    {
                        Debug.Console(1, this, "Making an awesome route : '{0}'", Key);
                        _videoInput.SetInput(VideoInputEnum.Hdmi1);
                        _audioInput.SetInput(AudioInputEnum.AudioFollowsVideo);
                    }),
                    this),
                new RoutingInputPort(
                    VideoInputEnum.Hdmi2.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    new Action(() =>
                    {
                        Debug.Console(1, this, "Making an awesome route : '{0}'", Key);
                        _videoInput.SetInput(VideoInputEnum.Hdmi2);
                        _audioInput.SetInput(AudioInputEnum.AudioFollowsVideo);
                    }),
                    this)
            });

            OutputPorts.Add(
                new RoutingOutputPort(
                    VideoOutputEnum.Hdmi.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    null,
                    this
                    ));
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var action = inputSelector as Action;
            if (action == null)
                throw new NullReferenceException("action");

            action();
        }

        public DmNvxBaseClass Hardware
        {
            get { return _videoInput.Hardware; }
        }

        public string Key
        {
            get { return _videoInput.Key; }
        }

        public string Name
        {
            get { return _videoInput.Name; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _videoInput.Feedbacks; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _videoInput.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _videoInput.OutputPorts; }
        }

        public UsageTracking UsageTracker
        {
            get { return _videoInput.UsageTracker; }
            set { _videoInput.UsageTracker = value; }
        }

        public int VirtualDeviceId
        {
            get { return _videoInput.VirtualDeviceId; }
        }

        public DeviceConfig Config
        {
            get { return _videoInput.Config; }
        }

        public BoolFeedback IsTransmitter
        {
            get { return _videoInput.IsTransmitter; }
        }

        public StringFeedback DeviceName
        {
            get { return _videoInput.DeviceName; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _videoInput.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _videoInput.VideoStreamStatus; }
        }

        public StringFeedback StreamUrl
        {
            get { return _videoInput.StreamUrl; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _videoInput.MulticastAddress; }
        }

        public StringFeedback VideoInputName
        {
            get { return _videoInput.VideoInputName; }
        }

        public IntFeedback VideoInputValue
        {
            get { return _videoInput.VideoInputValue; }
        }

        public StringFeedback AudioInputName
        {
            get { return _audioInput.AudioInputName; }
        }

        public IntFeedback AudioInputValue
        {
            get { return _audioInput.AudioInputValue; }
        }
    }
}