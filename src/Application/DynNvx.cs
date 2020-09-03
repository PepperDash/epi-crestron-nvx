using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Application.Builder;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models;
using NvxEpi.Device.Models.Entities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Application
{
    public class DynNvx : EssentialsBridgeableDevice
    {
        private Dictionary<int, INvxDevice> _transmitters;
        private Dictionary<int, INvxDevice> _receivers;

        public DynNvx(IDynNvxBuilder builder) : base(builder.Key)
        {
            AddPreActivationAction(() =>
            {
                _transmitters = builder
                    .Transmitters
                    .ToDictionary(x => x.Key, x => DeviceManager.GetDeviceForKey(x.Value) as INvxDevice);
            });

            AddPreActivationAction(() =>
            {
                _receivers = builder
                    .Receivers
                    .ToDictionary(x => x.Key, x => DeviceManager.GetDeviceForKey(x.Value) as INvxDevice);
            });

            AddPreActivationAction(() =>
            {
                foreach (var transmitter in _transmitters.Where(transmitter => transmitter.Value == null))
                {
                    Debug.Console(1, this, "Transmitter at input {0} is null!", transmitter.Key);
                    throw new NullReferenceException(transmitter.Key + " Is Null");
                }

                foreach (var transmitter in _transmitters.Where(transmitter => !transmitter.Value.IsTransmitter.BoolValue))
                {
                    Debug.Console(1, this, "Device at input {0} is not a transmitter!", transmitter.Key);
                    throw new ArgumentException(transmitter.Value.Key);
                }

                foreach (var receiver in _receivers.Where(receiver => receiver.Value == null))
                {
                    Debug.Console(1, this, "Receiver at output {0} is null!", receiver.Key);
                    throw new NullReferenceException(receiver.Key + " Is Null");
                }

                foreach (var receiver in _receivers.Where(receiver => receiver.Value.IsTransmitter.BoolValue))
                {
                    Debug.Console(1, this, "Device at output {0} is not a receiver!", receiver.Key);
                    throw new ArgumentException(receiver.Value.Key); ;
                }
            });
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new DmChassisControllerJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            LinkOnlineStatus(trilist, joinMap);
            LinkVideoRoutes(trilist, joinMap);
            LinkAudioRoutes(trilist, joinMap);

            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[NvxDevice.DeviceFeedbacks.Hdmi1SyncDetected.ToString()] as BoolFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                feedback.LinkInputSig(trilist.BooleanInput[joinMap.VideoSyncStatus.JoinNumber + index]);
            }

            foreach (var transmitter in _transmitters)
            {
                var feedback =
                    transmitter.Value.Feedbacks[NvxDevice.DeviceFeedbacks.DeviceName.ToString()] as StringFeedback;
                if (feedback == null)
                    continue;

                var index = (uint) transmitter.Key - 1;
                feedback.LinkInputSig(trilist.StringInput[joinMap.InputNames.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                var index = (uint) device.Key - 1;
                var feedback =
                    device.Value.Feedbacks[NvxDevice.DeviceFeedbacks.CurrentVideoRouteName.ToString()] as StringFeedback;
                if (feedback == null)
                    continue;

                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentVideoInputNames.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var index = (uint) device.Key - 1;
                var feedback =
                    device.Value.Feedbacks[NvxDevice.DeviceFeedbacks.CurrentAudioRouteName.ToString()] as StringFeedback;
                if (feedback == null)
                    continue;

                feedback.LinkInputSig(trilist.StringInput[joinMap.OutputCurrentAudioInputNames.JoinNumber + index]);
            }
        }

        private void LinkVideoRoutes(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var index = (uint) device.Key - 1;
                var rx = device.Value;

                var currentVideoRouteFeedback =
                    rx.Feedbacks[NvxDevice.DeviceFeedbacks.CurrentVideoRouteName.ToString()] as StringFeedback;

                if (currentVideoRouteFeedback == null)
                    continue;

                var feedback = new IntFeedback(rx.Key + "-CurrentVideoRouteNumber", () =>
                {
                    if (currentVideoRouteFeedback.StringValue.Equals(NvxRouter.Instance.NoSourceText)
                        || String.IsNullOrEmpty(currentVideoRouteFeedback.StringValue))
                        return 0;

                    var result =
                        _transmitters.FirstOrDefault(
                            x =>
                                x.Value.Name.Equals(currentVideoRouteFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return result.Value != null ? result.Key : 0;
                });

                feedback.OutputChange +=
                    (sender, args) => Debug.Console(1, this, "Video Output Change {0} | {1}", rx.Key, args.IntValue);

                currentVideoRouteFeedback.OutputChange += (sender, args) => feedback.FireUpdate();

                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputVideo.JoinNumber + index]);
                trilist.SetUShortSigAction(joinMap.OutputVideo.JoinNumber + index, source =>
                {
                    if (source == 0)
                        rx.StopVideoStream();

                    NvxDevice tx;
                    if (_transmitters.TryGetValue(source, out tx))
                        NvxRouter.RouteVideo(rx, tx);
                });
            }
        }

        private void LinkAudioRoutes(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var index = (uint) device.Key - 1;
                var rx = device.Value;

                var currentAudioRouteFeedback =
                    rx.Feedbacks[NvxDevice.DeviceFeedbacks.CurrentAudioRouteName.ToString()] as StringFeedback;

                if (currentAudioRouteFeedback == null)
                    continue;

                var feedback = new IntFeedback(rx.Key + "-CurrentAudioRouteNumber", () =>
                {
                    if (currentAudioRouteFeedback.StringValue.Equals(NvxRouter.Instance.NoSourceText)
                        || String.IsNullOrEmpty(currentAudioRouteFeedback.StringValue))
                        return 0;

                    var result =
                        _transmitters.FirstOrDefault(
                            x =>
                                x.Value.Name.Equals(currentAudioRouteFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return result.Value != null ? result.Key : 0;
                });

                feedback.OutputChange +=
                    (sender, args) => Debug.Console(1, this, "Audio Output Change {0} | {1}", rx.Key, args.IntValue);

                currentAudioRouteFeedback.OutputChange += (sender, args) => feedback.FireUpdate();

                feedback.LinkInputSig(trilist.UShortInput[joinMap.OutputAudio.JoinNumber + index]);
                trilist.SetUShortSigAction(joinMap.OutputAudio.JoinNumber + index, source =>
                {
                    if (source == 0)
                        rx.StopAudioStream();

                    NvxDevice tx;
                    if (_transmitters.TryGetValue(source, out tx))
                        NvxRouter.RouteAudio(rx, tx);
                });
            }
        }


        private void LinkOnlineStatus(BasicTriList trilist, DmChassisControllerJoinMap joinMap)
        {
            foreach (var device in _transmitters)
            {
                if (device.Value == null)
                    continue;

                var index = (uint) device.Key - 1;
                device.Value.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.InputEndpointOnline.JoinNumber + index]);
            }

            foreach (var device in _receivers)
            {
                if (device.Value == null)
                    continue;

                var index = (uint) device.Key - 1;
                device.Value.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.OutputEndpointOnline.JoinNumber + index]);
            }
        }
    }
}