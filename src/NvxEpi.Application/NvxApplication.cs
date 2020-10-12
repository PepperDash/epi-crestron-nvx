using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Application.Builder;
using NvxEpi.Application.JoinMap;
using NvxEpi.Application.Services;
using NvxEpi.Entities.Streams;
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
        private readonly Dictionary<int, INvxDevice> _transmitters = new Dictionary<int, INvxDevice>();
        private readonly Dictionary<int, INvxDevice> _receivers = new Dictionary<int, INvxDevice>();

        private readonly Dictionary<int, IRoutingSink> _videoDestinations = new Dictionary<int, IRoutingSink>();
        private readonly Dictionary<int, IRoutingSink> _audioDestinations = new Dictionary<int, IRoutingSink>();
        private readonly Dictionary<int, IRoutingSource> _sources = new Dictionary<int, IRoutingSource>();

        public bool AudioFollowVideo { get; private set; }

        public NvxApplication(IDynNvxBuilder builder) : base(builder.Key)
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

                    _transmitters.Add(item.Key, tx);
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
                    throw new ArgumentException(receiver.Value.Key); ;
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

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxApplicationJoinMap(joinStart);
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
            LinkHorizontalResolution(trilist, joinMap);
            LinkRxComPorts(trilist, joinMap);
        }

        private void LinkHorizontalResolution(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[HorizontalResolutionFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputHorizontalResolution.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[3301 + index]);
            }
        }

        private void LinkOutputDisabledFeedback(BasicTriList trilist, NvxApplicationJoinMap joinMap)
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

        private void LinkDeviceNames(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var transmitter in _transmitters)
            {
                var tx = transmitter.Value;
                var feedback =
                    tx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.InputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.InputNames.JoinNumber + index]);
                tx.VideoName.LinkInputSig(trilist.StringInput[joinMap.InputVideoNames.JoinNumber + index]);
                tx.AudioName.LinkInputSig(trilist.StringInput[joinMap.InputAudioNames.JoinNumber + index]);
            }

            foreach (var receiver in _receivers)
            {
                var rx = receiver.Value;
                var feedback =
                    rx.Feedbacks[DeviceNameFeedback.Key] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)receiver.Key - 1;
                Debug.Console(1, receiver.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputNames.JoinNumber + index]);
                rx.VideoName.LinkInputSig(trilist.StringInput[joinMap.OutputVideoNames.JoinNumber + index]);
                rx.AudioName.LinkInputSig(trilist.StringInput[joinMap.OutputAudioNames.JoinNumber + index]);
            }
        }

        private void LinkSyncDetectedStatus(BasicTriList trilist, NvxApplicationJoinMap joinMap)
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

        private void LinkHdcpCapability(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var transmitter in _transmitters)
            {
                var index = (uint)transmitter.Key - 1;
                var fb = new IntFeedback(() => 99);
                var advancedFb = new BoolFeedback(() => true);

                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", "HdcpCapability", joinMap.HdcpSupportCapability.JoinNumber + index);
                fb.LinkInputSig(trilist.UShortInput[joinMap.HdcpSupportCapability.JoinNumber + index]);
                advancedFb.LinkInputSig(trilist.BooleanInput[joinMap.TxAdvancedIsPresent.JoinNumber + index]);
                fb.FireUpdate();
                advancedFb.FireUpdate();
            }

            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[Hdmi1HdcpCapabilityValueFeedback.Key] as IntFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)transmitter.Key - 1;
                Debug.Console(1, transmitter.Value, "Linking Feedback:{0} to Join:{1}", "HdcpState", joinMap.HdcpSupportState.JoinNumber + index);
                feedback.LinkInputSig(trilist.UShortInput[joinMap.HdcpSupportState.JoinNumber + index]);
            }

            foreach (var transmitter in _transmitters)
            {
                if (transmitter.Value == null)
                    continue;

                var index = (uint)transmitter.Key - 1;
                var device = transmitter.Value as IHdmiInput;
                if (device == null) return;

                trilist.SetUShortSigAction(joinMap.HdcpSupportState.JoinNumber + index, state =>
                {
                    if (state == 99)
                        device.SetHdmi1HdcpCapability(eHdcpCapabilityType.HdcpAutoSupport);
                    else
                        device.SetHdmi1HdcpCapability(state);
                });
            }
        }

        private void LinkVideoRoutes(BasicTriList trilist, NvxApplicationJoinMap joinMap)
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

                var currentRouteFb = new IntFeedback("ApplicationOutputVideoNumber", 
                    () => _transmitters.FirstOrDefault(x => x.Value.VideoName.StringValue.Equals(feedback.StringValue)).Key);

                device.Value.Feedbacks.Add(currentRouteFb);
                feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", currentRouteFb.Key, joinMap.OutputVideo.JoinNumber + index);
                currentRouteFb.LinkInputSig(trilist.UShortInput[joinMap.OutputVideo.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var index = (uint) device.Key - 1;
                var dest = _videoDestinations[device.Key];

                Debug.Console(1, this, "Linking Video Route for Device:{0} to join {1}", device.Value.Name, joinMap.OutputVideo.JoinNumber + index);
                trilist.SetUShortSigAction(joinMap.OutputVideo.JoinNumber + index, source =>
                {
                    if (source == 0)
                    {
                        dest.ReleaseRoute();
                        return;
                    }
                        
                    IRoutingSource sourceToRoute;
                    if (_sources.TryGetValue(source, out sourceToRoute))
                        dest.ReleaseAndMakeRoute(sourceToRoute, AudioFollowVideo ? eRoutingSignalType.AudioVideo : eRoutingSignalType.Video);
                });
            }
        }

        private void LinkAudioRoutes(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                var feedback =
                    device.Value.Feedbacks[CurrentSecondaryAudioStream.RouteNameKey] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint)device.Key - 1;
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", feedback.Key, joinMap.OutputCurrentAudioInputNames.JoinNumber + index);
                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentAudioInputNames.JoinNumber + index]);

                var currentRouteFb = new IntFeedback("ApplicationOutputAudioNumber",
                    () => _transmitters.FirstOrDefault(x => x.Value.AudioName.StringValue.Equals(feedback.StringValue)).Key);

                device.Value.Feedbacks.Add(currentRouteFb);
                feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                Debug.Console(1, device.Value, "Linking Feedback:{0} to Join:{1}", currentRouteFb.Key, joinMap.OutputAudio.JoinNumber + index);
                currentRouteFb.LinkInputSig(trilist.UShortInput[joinMap.OutputVideo.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var index = (uint) device.Key - 1;
                var dest = _audioDestinations[device.Key];

                trilist.SetUShortSigAction(joinMap.OutputAudio.JoinNumber + index, source =>
                {
                    if (AudioFollowVideo)
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

        private void LinkOnlineStatus(BasicTriList trilist, NvxApplicationJoinMap joinMap)
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

        private void LinkRxComPorts(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                try
                {
                    var comDevice = device.Value as IComPorts;
                    if (comDevice == null || comDevice.ComPorts == null)
                        continue;

                    ComPort comPort;
                    if (!comDevice.ComPorts.TryGetValue(1, out comPort))
                        return;

                    var index = (uint)device.Key - 1;
                    Debug.Console(1, device.Value, "Linking Feedback:RX string transmit to Join:{0}", index + joinMap.ReceiverSerialPorts.JoinNumber);

                    comPort.SerialDataReceived +=
                        (port, args) => trilist.StringInput[index + joinMap.ReceiverSerialPorts.JoinNumber].StringValue = args.SerialData;

                    trilist.SetStringSigAction(index + joinMap.ReceiverSerialPorts.JoinNumber, tx => comPort.TransmitString = tx);
                }
                catch (NullReferenceException ex)
                {
                    Debug.Console(1, device.Value, "Error Linking Feedback:RX string transmit to Join:{0} | {1}", ex.Message, ex.InnerException);
                }
                catch (Exception ex)
                {
                    Debug.Console(1, device.Value, "Error Linking Feedback:RX string transmit to Join:{0} | {1}", ex.Message, ex.InnerException);
                }
            }
        }
    }
}