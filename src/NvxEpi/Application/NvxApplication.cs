using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Application.Builder;
using NvxEpi.Application.JoinMap;
using NvxEpi.Application.Services;
using NvxEpi.Entities.Routing;
using NvxEpi.Entities.Streams.Audio;
using NvxEpi.Entities.Streams.Video;
using NvxEpi.Extensions;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application
{
    public class NvxApplication : EssentialsBridgeableDevice
    {
        private readonly Dictionary<int, IRoutingSink> _audioDestinations = new Dictionary<int, IRoutingSink>();
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly Dictionary<int, INvxDevice> _receivers = new Dictionary<int, INvxDevice>();
        private readonly Dictionary<int, IRoutingSource> _sources = new Dictionary<int, IRoutingSource>();
        private readonly Dictionary<int, INvxDevice> _transmitters = new Dictionary<int, INvxDevice>();
        private readonly Dictionary<int, IRoutingSink> _videoDestinations = new Dictionary<int, IRoutingSink>();

        public NvxApplication(INvxApplicationBuilder applicationBuilder) : base(applicationBuilder.Key)
        {
            AddPreActivationAction(() =>
                {
                    foreach (var item in applicationBuilder.Transmitters)
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

                        _transmitters.Add(item.Key, tx);
                    }
                });

            AddPreActivationAction(() =>
                {
                    foreach (var item in applicationBuilder.Receivers)
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

                        _receivers.Add(item.Key, rx);
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
                        throw new ArgumentException(receiver.Value.Key);
                        ;
                    }
                });

            AddPreActivationAction(() =>
                {
                    foreach (var item in _receivers)
                    {
                        var id = item.Key;
                        var rx = item.Value;
                        var dest = new MockDisplay(rx.Key + "--Display", rx.Key + "--Display");
                        _videoDestinations.Add(id, dest);
                        ApplicationTieLineConnector.AddTieLineForMockDisplay(dest, rx);
                    }

                    foreach (var item in _receivers)
                    {
                        var id = item.Key;
                        var rx = item.Value;
                        var amp = new Amplifier(rx.Key + "--Amplifier", rx.Key + "--Amplifier");
                        _audioDestinations.Add(id, amp);
                        ApplicationTieLineConnector.AddTieLineForAmp(amp, rx);
                    }

                    foreach (var item in _transmitters)
                    {
                        var id = item.Key;
                        var tx = item.Value;
                        var source = new DummyRoutingInputsDevice(tx.Key + "--Source");
                        _sources.Add(id, source);
                        ApplicationTieLineConnector.AddTieLineForDummySource(source, tx);
                    }
                });
        }

        public bool EnableAudioBreakaway { get; private set; }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxApplicationJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            trilist.SetBoolSigAction(joinMap.EnableAudioBreakaway.JoinNumber,
                value =>
                    {
                        if (EnableAudioBreakaway == value)
                            return;

                        try
                        {
                            _lock.Enter();
                            EnableAudioBreakaway = value;
                            Debug.Console(1, this, "Setting EnableAudioBreakaway to : {0}", EnableAudioBreakaway);

                            var audioFollowsVideoHandler = new AudioFollowsVideoHandler(_transmitters, _receivers);
                            if (EnableAudioBreakaway)
                                audioFollowsVideoHandler.SetAudioFollowsVideoFalse();
                            else
                                audioFollowsVideoHandler.SetAudioFollowsVideoTrue();
                        }
                        finally
                        {
                            _lock.Leave();
                        }
                    });

            LinkOnlineStatus(trilist, joinMap);
            LinkVideoRoutes(trilist, joinMap);
            LinkAudioRoutes(trilist, joinMap);
            LinkSyncDetectedStatus(trilist, joinMap);
            LinkDeviceNames(trilist, joinMap);
            LinkHdcpCapability(trilist, joinMap);
            LinkOutputDisabledFeedback(trilist, joinMap);
            LinkHorizontalResolution(trilist, joinMap);
            LinkRxComPorts(trilist, joinMap);
        }

        private void LinkAudioRoutes(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.OutputCurrentAudioInputNames.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_receivers.TryGetValue(x, out rx))
                    continue;

                var feedback =
                    rx.Feedbacks[CurrentSecondaryAudioStream.RouteNameKey] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputCurrentAudioInputNames.JoinNumber + index);

                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentAudioInputNames.JoinNumber + index]);

                var currentRouteFb = new IntFeedback(() =>
                    {
                        if (feedback.StringValue.Equals(NvxGlobalRouter.NoSourceText))
                            return 0;

                        return _transmitters
                            .Where(
                                t =>
                                    t.Value.AudioName.StringValue.Equals(feedback.StringValue,
                                        StringComparison.OrdinalIgnoreCase))
                            .Select(t => t.Key)
                            .FirstOrDefault();
                    });

                rx.Feedbacks.Add(currentRouteFb);
                feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    currentRouteFb.Key,
                    joinMap.OutputAudio.JoinNumber + index);

                currentRouteFb.LinkInputSig(trilist.UShortInput[joinMap.OutputAudio.JoinNumber + index]);
            }

            for (var x = 1; x <= joinMap.OutputAudio.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_receivers.TryGetValue(x, out rx))
                    continue;

                var index = (uint) x - 1;
                var dest = _audioDestinations[x];

                trilist.SetUShortSigAction(joinMap.OutputAudio.JoinNumber + index,
                    source =>
                        {
                            if (!EnableAudioBreakaway)
                                return;

                            if (source == 0)
                            {
                                dest.ReleaseRoute();
                                return;
                            }

                            IRoutingSource sourceToRoute;
                            if (_sources.TryGetValue(source, out sourceToRoute))
                                dest.ReleaseAndMakeRoute(sourceToRoute, eRoutingSignalType.Audio);
                        });
            }
        }

        private void LinkDeviceNames(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.InputNames.JoinSpan; x++)
            {
                INvxDevice tx;
                if (!_transmitters.TryGetValue(x, out tx))
                    continue;

                var feedback =
                    tx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    tx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.InputNames.JoinNumber + index);

                feedback.LinkInputSig(trilist.StringInput[joinMap.InputNames.JoinNumber + index]);
                tx.VideoName.LinkInputSig(trilist.StringInput[joinMap.InputVideoNames.JoinNumber + index]);
                tx.AudioName.LinkInputSig(trilist.StringInput[joinMap.InputAudioNames.JoinNumber + index]);
            }

            for (var x = 1; x <= joinMap.OutputNames.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_receivers.TryGetValue(x, out rx))
                    continue;

                var feedback =
                    rx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputNames.JoinNumber + index);

                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputNames.JoinNumber + index]);
                rx.VideoName.LinkInputSig(trilist.StringInput[joinMap.OutputVideoNames.JoinNumber + index]);
                rx.AudioName.LinkInputSig(trilist.StringInput[joinMap.OutputAudioNames.JoinNumber + index]);
            }
        }

        private void LinkHdcpCapability(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.HdcpSupportCapability.JoinSpan; x++)
            {
                INvxDevice transmitter;
                if (!_transmitters.TryGetValue(x, out transmitter))
                    continue;

                var index = (uint) x - 1;
                var fb = new IntFeedback(() => 99);
                var advancedFb = new BoolFeedback(() => true);

                Debug.Console(1,
                    transmitter,
                    "Linking Feedback:{0} to Join:{1}",
                    "HdcpCapability",
                    joinMap.HdcpSupportCapability.JoinNumber + index);

                fb.LinkInputSig(trilist.UShortInput[joinMap.HdcpSupportCapability.JoinNumber + index]);
                advancedFb.LinkInputSig(trilist.BooleanInput[joinMap.TxAdvancedIsPresent.JoinNumber + index]);
                fb.FireUpdate();
                advancedFb.FireUpdate();
            }

            for (var x = 1; x <= joinMap.HdcpSupportState.JoinSpan; x++)
            {
                INvxDevice transmitter;
                if (!_transmitters.TryGetValue(x, out transmitter))
                    continue;

                var feedback =
                    transmitter.Feedbacks[Hdmi1HdcpCapabilityValueFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    transmitter,
                    "Linking Feedback:{0} to Join:{1}",
                    "HdcpState",
                    joinMap.HdcpSupportState.JoinNumber + index);

                feedback.LinkInputSig(trilist.UShortInput[joinMap.HdcpSupportState.JoinNumber + index]);
            }

            for (var x = 1; x <= joinMap.HdcpSupportState.JoinSpan; x++)
            {
                INvxDevice transmitter;
                if (!_transmitters.TryGetValue(x, out transmitter))
                    continue;

                var index = (uint) x - 1;
                var device = transmitter as IHdmiInput;
                if (device == null) return;

                trilist.SetUShortSigAction(joinMap.HdcpSupportState.JoinNumber + index,
                    state =>
                        {
                            if (state == 99)
                                device.SetHdmi1HdcpCapability(eHdcpCapabilityType.HdcpAutoSupport);
                            else
                                device.SetHdmi1HdcpCapability(state);
                        });
            }
        }

        private void LinkHorizontalResolution(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.OutputHorizontalResolution.JoinSpan; x++)
            {
                INvxDevice device;
                if (!_transmitters.TryGetValue(x, out device))
                    continue;

                var feedback =
                    device.Feedbacks[HorizontalResolutionFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    device,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputHorizontalResolution.JoinNumber + index);

                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputHorizontalResolution.JoinNumber + index]);
            }
        }

        private void LinkOnlineStatus(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.InputEndpointOnline.JoinSpan; x++)
            {
                INvxDevice transmitter;
                if (!_transmitters.TryGetValue(x, out transmitter))
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    transmitter,
                    "Linking Feedback:DeviceOnline to Join:{0}",
                    joinMap.InputEndpointOnline.JoinNumber + index);
                transmitter.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.InputEndpointOnline.JoinNumber + index]);
            }

            for (var x = 1; x <= joinMap.OutputEndpointOnline.JoinSpan; x++)
            {
                INvxDevice receiver;
                if (!_receivers.TryGetValue(x, out receiver))
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    receiver,
                    "Linking Feedback:DeviceOnline to Join:{0}",
                    joinMap.OutputEndpointOnline.JoinNumber + index);
                receiver.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.OutputEndpointOnline.JoinNumber + index]);
            }
        }

        private void LinkOutputDisabledFeedback(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.OutputDisabledByHdcp.JoinSpan; x++)
            {
                INvxDevice device;
                if (!_receivers.TryGetValue(x, out device))
                    continue;

                var feedback =
                    device.Feedbacks[HdmiOutputDisabledFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    device,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputHorizontalResolution.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputHorizontalResolution.JoinNumber + index]);
            }
        }

        private void LinkRxComPorts(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.ReceiverSerialPorts.JoinSpan; x++)
            {
                INvxDevice receiver;
                if (!_receivers.TryGetValue(x, out receiver))
                    continue;

                var comDevice = receiver as IComPorts;
                if (comDevice == null || comDevice.ComPorts == null)
                    continue;

                ComPort comPort;
                if (!comDevice.ComPorts.TryGetValue(1, out comPort))
                    return;

                var index = (uint) x - 1;
                Debug.Console(1,
                    receiver,
                    "Linking Feedback:RX string transmit to Join:{0}",
                    index + joinMap.ReceiverSerialPorts.JoinNumber);

                comPort.SerialDataReceived +=
                    (port, args) =>
                        trilist.StringInput[index + joinMap.ReceiverSerialPorts.JoinNumber].StringValue =
                            args.SerialData;

                trilist.SetStringSigAction(index + joinMap.ReceiverSerialPorts.JoinNumber,
                    tx => comPort.TransmitString = tx);
            }
        }

        private void LinkSyncDetectedStatus(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.VideoSyncStatus.JoinSpan; x++)
            {
                INvxDevice transmitter;
                if (!_transmitters.TryGetValue(x, out transmitter))
                    continue;

                var feedback =
                    transmitter.Feedbacks[Hdmi1SyncDetectedFeedback.Key] as BoolFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    transmitter,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.VideoSyncStatus.JoinNumber + index);

                feedback.LinkInputSig(trilist.BooleanInput[joinMap.VideoSyncStatus.JoinNumber + index]);
            }
        }

        private void LinkVideoRoutes(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.OutputCurrentVideoInputNames.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_receivers.TryGetValue(x, out rx))
                    continue;

                var feedback =
                    rx.Feedbacks[CurrentVideoStream.RouteNameKey] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) x - 1;
                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputCurrentVideoInputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentVideoInputNames.JoinNumber + index]);

                var currentRouteFb = new IntFeedback(() =>
                    {
                        if (feedback.StringValue.Equals(NvxGlobalRouter.NoSourceText))
                            return 0;

                        return _transmitters
                            .Where(
                                t =>
                                    t.Value.VideoName.StringValue.Equals(feedback.StringValue,
                                        StringComparison.OrdinalIgnoreCase))
                            .Select(t => t.Key)
                            .FirstOrDefault();
                    });

                rx.Feedbacks.Add(currentRouteFb);
                feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();

                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    currentRouteFb.Key,
                    joinMap.OutputVideo.JoinNumber + index);
                currentRouteFb.LinkInputSig(trilist.UShortInput[joinMap.OutputVideo.JoinNumber + index]);
            }

            for (var x = 1; x <= joinMap.OutputVideo.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_receivers.TryGetValue(x, out rx))
                    continue;

                var index = (uint) x - 1;
                var dest = _videoDestinations[x];

                Debug.Console(1, rx, "Linking Video Route for to join {0}", joinMap.OutputVideo.JoinNumber + index);
                trilist.SetUShortSigAction(joinMap.OutputVideo.JoinNumber + index,
                    source =>
                        {
                            if (source == 0)
                            {
                                dest.ReleaseRoute();
                                return;
                            }

                            IRoutingSource sourceToRoute;
                            if (_sources.TryGetValue(source, out sourceToRoute))
                            {
                                dest.ReleaseAndMakeRoute(sourceToRoute,
                                    EnableAudioBreakaway ? eRoutingSignalType.Video : eRoutingSignalType.AudioVideo);
                            }
                        });
            }
        }
    }
}