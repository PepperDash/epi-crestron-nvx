using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Application.Builder;
using NvxEpi.Application.Entities;
using NvxEpi.Application.JoinMap;
using NvxEpi.Application.Services;
using NvxEpi.Extensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Application
{
    public class NvxApplication : EssentialsBridgeableDevice
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly Dictionary<int, NvxApplicationVideoTransmitter> _transmitters;
        private readonly Dictionary<int, NvxApplicationVideoReceiver> _receivers;

        public NvxApplication(INvxApplicationBuilder applicationBuilder) : base(applicationBuilder.Key)
        {
            _transmitters =
                applicationBuilder.Transmitters.Select(
                    x => new NvxApplicationVideoTransmitter(x.Value.DeviceKey + "--device", x.Value, x.Key))
                                  .ToDictionary(x => x.DeviceId);

            _transmitters
                .Values
                .ToList()
                .ForEach(DeviceManager.AddDevice);

            _receivers =
                applicationBuilder.Receivers.Select(
                    x => new NvxApplicationVideoReceiver(x.Value.DeviceKey + "--device", x.Value, x.Key, _transmitters.Values))
                                  .ToDictionary(x => x.DeviceId);

            _receivers
                .Values.
                ToList()
                .ForEach(DeviceManager.AddDevice);
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

                            var audioFollowsVideoHandler = new AudioFollowsVideoHandler(
                                _transmitters.ToDictionary(x => x.Key, x => x.Value.Device), _receivers.ToDictionary(x => x.Key, x => x.Value.Device));

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

            LinkTransmitters(trilist, joinMap);
            LinkReceivers(trilist, joinMap);
        }

        private void LinkReceivers(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _receivers.Select(x => new {DeviceId = x.Key, DeviceActual = x.Value}))
            {
                item.DeviceActual.IsOnline.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.OutputEndpointOnline.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.NameFeedback.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputNames.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.VideoName.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputVideoNames.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.DisabledByHdcp.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.OutputDisabledByHdcp.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.HorizontalResolution.LinkInputSig(
                    trilist.UShortInput[(uint) ( joinMap.OutputHorizontalResolution.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.EdidManufacturer.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputEdidManufacturer.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.CurrentVideoRouteId.LinkInputSig(
                    trilist.UShortInput[(uint) ( joinMap.OutputVideo.JoinNumber + item.DeviceId - 1 )]);
                item.DeviceActual.CurrentVideoRouteName.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputCurrentVideoInputNames.JoinNumber + item.DeviceId - 1 )]);

                var rx = item.DeviceActual;
                trilist.SetUShortSigAction((uint) ( joinMap.OutputVideo.JoinNumber + item.DeviceId - 1 ),
                    s =>
                        {
                            if (s == 0)
                                rx.Display.ReleaseRoute();
                            else
                            {
                                NvxApplicationVideoTransmitter device;
                                if (!_transmitters.TryGetValue(s, out device))
                                    return;

                                rx.Display.ReleaseAndMakeRoute(device.Source, EnableAudioBreakaway ? eRoutingSignalType.Video : eRoutingSignalType.AudioVideo);   
                            }
                        });
            }
        }

        private void LinkTransmitters(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _transmitters.Select(x => new {DeviceId = x.Key, DeviceActual = x.Value}))
            {
                Debug.Console(1, this, "Linking {0} Online to join {1}", item.DeviceActual.Key, joinMap.InputEndpointOnline.JoinNumber + item.DeviceId - 1 );
                item.DeviceActual.IsOnline.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.InputEndpointOnline.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(1, this, "Linking {0} Name to join {1}", item.DeviceActual.Key, joinMap.InputNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.NameFeedback.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.InputNames.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(1, this, "Linking {0} Video Name to join {1}", item.DeviceActual.Key, joinMap.InputVideoNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.VideoName.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.InputVideoNames.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(1, this, "Linking {0} Input Resolution to join {1}", item.DeviceActual.Key, joinMap.InputCurrentResolution.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.InputResolution.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.InputCurrentResolution.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(1, this, "Linking {0} VideoSyncStatus to join {1}", item.DeviceActual.Key, joinMap.VideoSyncStatus.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HdmiSyncDetected.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.VideoSyncStatus.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(1, this, "Linking {0} HdcpSupportCapability to join {1}", item.DeviceActual.Key, joinMap.HdcpSupportCapability.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HdcpCapability.LinkInputSig(
                    trilist.UShortInput[(uint) ( joinMap.HdcpSupportCapability.JoinNumber + item.DeviceId - 1 )]);

                trilist.SetUShortSigAction((uint) ( joinMap.HdcpSupportCapability.JoinNumber + item.DeviceId - 1 ),
                    item.DeviceActual.SetHdcpCapability);
            }
        }

        /*private void LinkAudioRoutes(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            for (var x = 1; x <= joinMap.OutputCurrentAudioInputNames.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_audioReceivers.TryGetValue(x, out rx))
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

                        return _audioTransmitters
                            .Where(
                                t =>
                                    t.Value.AudioSourceName.StringValue.Equals(feedback.StringValue,
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
                if (!_audioReceivers.TryGetValue(x, out rx))
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
                            if (_audioSources.TryGetValue(source, out sourceToRoute))
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
            }

            for (var x = 1; x <= joinMap.InputAudioNames.JoinSpan; x++)
            {
                INvxDevice tx;
                if (!_audioTransmitters.TryGetValue(x, out tx))
                    continue;

                var feedback =
                    tx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)x - 1;
                Debug.Console(1,
                    tx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.InputAudioNames.JoinNumber + index);

                tx.AudioSourceName.LinkInputSig(trilist.StringInput[joinMap.InputAudioNames.JoinNumber + index]);
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
            }

            for (var x = 1; x <= joinMap.OutputAudioNames.JoinSpan; x++)
            {
                INvxDevice rx;
                if (!_audioReceivers.TryGetValue(x, out rx))
                    continue;

                var feedback =
                    rx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)x - 1;
                Debug.Console(1,
                    rx,
                    "Linking Feedback:{0} to Join:{1}",
                    feedback.Key,
                    joinMap.OutputAudioNames.JoinNumber + index);

                rx.AudioSourceName.LinkInputSig(trilist.StringInput[joinMap.OutputAudioNames.JoinNumber + index]);
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
                if (!_receivers.TryGetValue(x, out device))
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
                            if (_videoSources.TryGetValue(source, out sourceToRoute))
                            {
                                dest.ReleaseAndMakeRoute(sourceToRoute,
                                    EnableAudioBreakaway ? eRoutingSignalType.Video : eRoutingSignalType.AudioVideo);
                            }
                        });
            }
        }*/
    }
}