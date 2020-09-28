using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Application.Builder;
using NvxEpi.Device.Entities.Routing;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application
{
    public class DynNvx : EssentialsBridgeableDevice
    {
        private readonly Dictionary<int, INvxDevice> _transmitters = new Dictionary<int, INvxDevice>();
        private readonly Dictionary<int, INvxDevice> _receivers = new Dictionary<int, INvxDevice>();

        private readonly Dictionary<int, MockDisplay> _videoDestinations = new Dictionary<int, MockDisplay>();
        private readonly Dictionary<int, Amplifier> _audioDestinations = new Dictionary<int, Amplifier>();
        private readonly Dictionary<int, DummyRoutingInputsDevice> _sources = new Dictionary<int, DummyRoutingInputsDevice>();

        public bool AudioFollowVideo { get; private set; }

        public DynNvx(IDynNvxBuilder builder) : base(builder.Key)
        {
            AddPreActivationAction(() =>
            {
                foreach (var item in builder.Transmitters)
                {
                    var tx = DeviceManager.GetDeviceForKey(item.Value) as INvxDevice;
                    if (tx == null)
                    {
                        Debug.Console(1, this, "Could not get TX : {0}", item.Value);
                        continue;
                    }

                    if (!tx.IsTransmitter)
                    {
                        Debug.Console(1, this, "Device is not a TX : {0}", item.Value);
                        continue;
                    }

                    tx.UpdateDeviceId((uint)item.Key);
                    _transmitters.Add(tx.DeviceId, tx);
                }
            });

            AddPreActivationAction(() =>
            {
                foreach (var item in builder.Receivers)
                {
                    var rx = DeviceManager.GetDeviceForKey(item.Value) as INvxDevice;
                    if (rx == null)
                    {
                        Debug.Console(1, this, "Could not get RX : {0}", item.Value);
                        continue;
                    }

                    if (rx.IsTransmitter)
                    {
                        Debug.Console(1, this, "Device is not a RX : {0}", item.Value);
                        continue;
                    }

                    rx.UpdateDeviceId((uint)item.Key);
                    _receivers.Add(rx.DeviceId, rx);
                }
            });

            AddPreActivationAction(() =>
            {
                foreach (var transmitter in _transmitters.Where(transmitter => transmitter.Value == null))
                {
                    Debug.Console(1, this, "Transmitter at input {0} is null!", transmitter.Key);
                    throw new NullReferenceException(transmitter.Key + " Is Null");
                }

                foreach (var transmitter in _transmitters.Where(transmitter => !transmitter.Value.IsTransmitter))
                {
                    Debug.Console(1, this, "Device at input {0} is not a transmitter!", transmitter.Key);
                    throw new ArgumentException(transmitter.Value.Key);
                }

                foreach (var receiver in _receivers.Where(receiver => receiver.Value == null))
                {
                    Debug.Console(1, this, "Receiver at output {0} is null!", receiver.Key);
                    throw new NullReferenceException(receiver.Key + " Is Null");
                }

                foreach (var receiver in _receivers.Where(receiver => receiver.Value.IsTransmitter))
                {
                    Debug.Console(1, this, "Device at output {0} is not a receiver!", receiver.Key);
                    throw new ArgumentException(receiver.Value.Key); ;
                }
            });

            AddPreActivationAction(() =>
            {
                foreach (var item in _receivers)
                {
                    var id = item.Key;
                    var rx = item.Value;
                    _videoDestinations.Add(id, new MockDisplay(rx.Key + "--Display", rx.Key + "--Display"));
                }

                foreach (var item in _receivers)
                {
                    var id = item.Key;
                    var rx = item.Value;
                    _audioDestinations.Add(id, new Amplifier(rx.Key + "--Amplifier", rx.Key + "--Amplifier"));
                }

                foreach (var item in _transmitters)
                {
                    var id = item.Key;
                    var tx = item.Value;
                    _sources.Add(id, new DummyRoutingInputsDevice(tx.Key + "--Source"));
                }
            });

            AddPostActivationAction(() => TieLineConnector.AddTieLinesForTransmitters(_transmitters.Values));
            AddPostActivationAction(() => TieLineConnector.AddTieLinesForReceivers(_receivers.Values));

            AddPostActivationAction(() => TieLineConnector.AddTieLinesForVideoDestinations(
                new ReadOnlyDictionary<int, MockDisplay>(_videoDestinations), 
                new ReadOnlyDictionary<int, INvxDevice>(_receivers))
                );

            AddPostActivationAction(() => TieLineConnector.AddTieLinesForAudioDestinations(
                new ReadOnlyDictionary<int, Amplifier>(_audioDestinations),
                new ReadOnlyDictionary<int, INvxDevice>(_receivers))
                );

            AddPostActivationAction(() => TieLineConnector.AddTieLinesForSources(
                new ReadOnlyDictionary<int, DummyRoutingInputsDevice>(_sources),
                new ReadOnlyDictionary<int, INvxDevice>(_transmitters))
                );

            AddPostActivationAction(AddNoneInputToRouters);
        }

        private void AddNoneInputToRouters()
        {
            var none = new DummyRoutingInputsDevice("None");
            _sources.Add(0, none);

            TieLineCollection.Default.Add(new TieLine(none.AudioVideoOutputPort, NvxGlobalRouter
                .Instance
                .PrimaryStreamRouter
                .Off, eRoutingSignalType.AudioVideo));

            TieLineCollection.Default.Add(new TieLine(none.AudioVideoOutputPort, NvxGlobalRouter
                .Instance
                .SecondaryAudioRouter
                .Off, eRoutingSignalType.Audio));
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new DmChassisControllerJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            trilist.SetBoolSigAction(joinMap.EnableAudioBreakaway.JoinNumber, value => AudioFollowVideo = !value);
            LinkOnlineStatus(trilist, joinMap);
            LinkVideoRoutes(trilist, joinMap);
            LinkAudioRoutes(trilist, joinMap);
            LinkSyncDetectedStatus(trilist, joinMap);
            LinkDeviceNames(trilist, joinMap);
            LinkHdcpCapability(trilist, joinMap);
            LinkOutputDisabledFeedback(trilist, joinMap);
            LinkHorizontalResolution(trilist);
        }

        private void LinkHorizontalResolution(BasicTriList trilist)
        {
            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[HorizontalResolutionFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, 3301 + index);
                feedback.LinkInputSig(trilist.UShortInput[3301 + index]);
            }
        }

        private void LinkOutputDisabledFeedback(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[HdmiOutputDisabledFeedback.Key] as BoolFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputDisabledByHdcp.JoinNumber + index);
                feedback.LinkInputSig(trilist.BooleanInput[joinMap.OutputDisabledByHdcp.JoinNumber + index]);
            }
        }

        private void LinkDeviceNames(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.InputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.InputNames.JoinNumber + index]);
            }

            foreach (var transmitter in _receivers)
            {
                var feedback =
                    transmitter.Value.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputNames.JoinNumber + index]);
            }
        }

        private void LinkSyncDetectedStatus(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[Hdmi1SyncDetectedFeedback.Key] as BoolFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.VideoSyncStatus.JoinNumber + index);
                feedback.LinkInputSig(trilist.BooleanInput[joinMap.VideoSyncStatus.JoinNumber + index]);
            }
        }

        private void LinkHdcpCapability(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[Hdmi1HdcpCapabilityValueFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.HdcpSupportCapability.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[joinMap.HdcpSupportCapability.JoinNumber + index]);
            }

            foreach (var transmitter in _transmitters)
            {
                if (transmitter.Value == null)
                    continue;

                var index = (uint)transmitter.Key - 1;
                var device = transmitter.Value as IHdmiInput;
                if (device == null) return;

                trilist.SetUShortSigAction(joinMap.HdcpSupportCapability.JoinNumber + index, device.SetHdmi1HdcpCapability);
            }
        }

        private void LinkVideoRoutes(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[CurrentVideoStream.RouteNameKey] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputCurrentVideoInputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentVideoInputNames.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[CurrentVideoStream.RouteValueKey] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputVideo.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputVideo.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var index = (uint) device.Key - 1;
                var dest = _videoDestinations[device.Key];

                Debug.Console(0, this, "Linking Video Route for Device:{0} to join {1}", device.Value.Name, joinMap.OutputVideo.JoinNumber + index);
                trilist.SetUShortSigAction(joinMap.OutputVideo.JoinNumber + index, source =>
                {
                    DummyRoutingInputsDevice sourceToRoute;
                    if (_sources.TryGetValue(source, out sourceToRoute))
                        dest.ReleaseAndMakeRoute(sourceToRoute, AudioFollowVideo ? eRoutingSignalType.AudioVideo : eRoutingSignalType.Video);
                });
            }
        }

        private void LinkAudioRoutes(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var feedback =
                    device.Value.Feedbacks[CurrentSecondaryAudioStream.RouteNameKey] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputCurrentAudioInputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentAudioInputNames.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[CurrentSecondaryAudioStream.RouteValueKey] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputAudio.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputAudio.JoinNumber + index]);
            }
            foreach (var device in _receivers)
            {
                var index = (uint) device.Key - 1;
                var dest = _audioDestinations[device.Key];

                trilist.SetUShortSigAction(joinMap.OutputAudio.JoinNumber + index, source =>
                {
                    if (AudioFollowVideo)
                        return;
                    
                    DummyRoutingInputsDevice sourceToRoute;
                    if (_sources.TryGetValue(source, out sourceToRoute))
                        dest.ReleaseAndMakeRoute(sourceToRoute, eRoutingSignalType.Audio);
                });
            }
        }

        private void LinkOnlineStatus(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _transmitters)
            {
                if (device.Value == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:DeviceOnline to Join:{0}", joinMap.InputEndpointOnline.JoinNumber + index);
                device.Value.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.InputEndpointOnline.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:DeviceOnline to Join:{0}", joinMap.OutputEndpointOnline.JoinNumber + index);
                device.Value.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.OutputEndpointOnline.JoinNumber + index]);
            }
        }
    }
}