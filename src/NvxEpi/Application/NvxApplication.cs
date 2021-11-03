using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.SecondaryAudio;
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
        private readonly Dictionary<int, NvxApplicationAudioTransmitter> _audioTransmitters;
        private readonly Dictionary<int, NvxApplicationAudioReceiver> _audioReceivers;

        public NvxApplication(INvxApplicationBuilder applicationBuilder) : base(applicationBuilder.Key)
        {
            _transmitters =
                applicationBuilder.Transmitters.Select(
                    x => new NvxApplicationVideoTransmitter(x.Value.DeviceKey + "--VideoRoutingTx", x.Value, x.Key))
                                  .ToDictionary(x => x.DeviceId);

            _transmitters
                .Values
                .ToList()
                .ForEach(DeviceManager.AddDevice);

            _receivers =
                applicationBuilder.Receivers.Select(
                    x => new NvxApplicationVideoReceiver(x.Value.DeviceKey + "--VideoRoutingRx", x.Value, x.Key, _transmitters.Values))
                                  .ToDictionary(x => x.DeviceId);

            _receivers
                .Values
                .ToList()
                .ForEach(DeviceManager.AddDevice);

            _audioTransmitters =
                applicationBuilder.AudioTransmitters.Select(
                    x => new NvxApplicationAudioTransmitter(x.Value.DeviceKey + "--AudioRoutingTx", x.Value, x.Key))
                                  .ToDictionary(x => x.DeviceId);

            _audioTransmitters
                .Values
                .ToList()
                .ForEach(DeviceManager.AddDevice);

            _audioReceivers =
                applicationBuilder.AudioReceivers.Select(
                    x => new NvxApplicationAudioReceiver(x.Value.DeviceKey + "--AudioRoutingRx", x.Value, x.Key, _audioTransmitters.Values))
                                  .ToDictionary(x => x.DeviceId);

            _audioReceivers
                .Values
                .ToList()
                .ForEach(DeviceManager.AddDevice);

            _enableAudioBreakawayFeedback = new BoolFeedback(() => _enableAudioBreakaway);
        }

        private bool _enableAudioBreakaway;
        private readonly BoolFeedback _enableAudioBreakawayFeedback;

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxApplicationJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            _enableAudioBreakawayFeedback.LinkInputSig(trilist.BooleanInput[joinMap.EnableAudioBreakaway.JoinNumber]);
            trilist.SetBoolSigAction(joinMap.EnableAudioBreakaway.JoinNumber,
                value =>
                    {
                        if (_enableAudioBreakaway == value)
                            return;

                        try
                        {
                            _lock.Enter();
                            _enableAudioBreakaway = value;
                            Debug.Console(1, this, "Setting EnableAudioBreakaway to : {0}", _enableAudioBreakaway);

                            var audioFollowsVideoHandler = new AudioFollowsVideoHandler(
                                _transmitters.ToDictionary(x => x.Key, x => x.Value.Device), _receivers.ToDictionary(x => x.Key, x => x.Value.Device));

                            if (_enableAudioBreakaway)
                                audioFollowsVideoHandler.SetAudioFollowsVideoFalse();
                            else
                                audioFollowsVideoHandler.SetAudioFollowsVideoTrue();
                        }
                        finally
                        {
                            _lock.Leave();
                            _enableAudioBreakawayFeedback.FireUpdate();
                        }
                    });

            LinkTransmitters(trilist, joinMap);
            LinkReceivers(trilist, joinMap);
            LinkAudioTransmitters(trilist, joinMap);
            LinkAudioReceivers(trilist, joinMap);
        }

        private void LinkTransmitters(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _transmitters.Select(x => new { DeviceId = x.Key, DeviceActual = x.Value }))
            {
                Debug.Console(2, this, "Linking {0} Online to join {1}", item.DeviceActual.Key, joinMap.InputEndpointOnline.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.IsOnline.LinkInputSig(
                    trilist.BooleanInput[(uint)(joinMap.InputEndpointOnline.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} Name to join {1}", item.DeviceActual.Key, joinMap.InputNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.NameFeedback.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.InputNames.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} Video Name to join {1}", item.DeviceActual.Key, joinMap.InputVideoNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.VideoName.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.InputVideoNames.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} Input Resolution to join {1}", item.DeviceActual.Key, joinMap.InputCurrentResolution.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.InputResolution.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.InputCurrentResolution.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} VideoSyncStatus to join {1}", item.DeviceActual.Key, joinMap.VideoSyncStatus.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HdmiSyncDetected.LinkInputSig(
                    trilist.BooleanInput[(uint)(joinMap.VideoSyncStatus.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} HdcpSupportCapability to join {1}", item.DeviceActual.Key, joinMap.HdcpSupportCapability.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HdcpCapability.LinkInputSig(
                    trilist.UShortInput[(uint)(joinMap.HdcpSupportCapability.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} HdcpSupportState to join {1}", item.DeviceActual.Key, joinMap.HdcpSupportState.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HdcpState.LinkInputSig(
                    trilist.UShortInput[(uint)(joinMap.HdcpSupportState.JoinNumber + item.DeviceId - 1)]);

                trilist.SetUShortSigAction((uint)(joinMap.HdcpSupportState.JoinNumber + item.DeviceId - 1),
                    item.DeviceActual.SetHdcpState);
            }
        }

        private void LinkReceivers(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _receivers.Select(x => new {DeviceId = x.Key, DeviceActual = x.Value}))
            {
                Debug.Console(2, this, "Linking {0} Online to join {1}", item.DeviceActual.Key, joinMap.OutputEndpointOnline.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.IsOnline.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.OutputEndpointOnline.JoinNumber + item.DeviceId - 1 )]);
 

                Debug.Console(2, this, "Linking {0} Name to join {1}", item.DeviceActual.Key, joinMap.OutputNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.NameFeedback.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputNames.JoinNumber + item.DeviceId - 1 )]);


                Debug.Console(2, this, "Linking {0} OutputVideoNames to join {1}", item.DeviceActual.Key, joinMap.OutputVideoNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.VideoName.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputVideoNames.JoinNumber + item.DeviceId - 1 )]);


                Debug.Console(2, this, "Linking {0} OutputDisabledByHdcp to join {1}", item.DeviceActual.Key, joinMap.OutputDisabledByHdcp.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.DisabledByHdcp.LinkInputSig(
                    trilist.BooleanInput[(uint) ( joinMap.OutputDisabledByHdcp.JoinNumber + item.DeviceId - 1 )]);


                Debug.Console(2, this, "Linking {0} OutputHorizontalResolution to join {1}", item.DeviceActual.Key, joinMap.OutputHorizontalResolution.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.HorizontalResolution.LinkInputSig(
                    trilist.UShortInput[(uint) ( joinMap.OutputHorizontalResolution.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(2, this, "Linking {0} OutputEdidManufacturer to join {1}", item.DeviceActual.Key, joinMap.OutputEdidManufacturer.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.EdidManufacturer.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputEdidManufacturer.JoinNumber + item.DeviceId - 1 )]);


                Debug.Console(2, this, "Linking {0} OutputVideo to join {1}", item.DeviceActual.Key, joinMap.OutputVideo.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.CurrentVideoRouteId.LinkInputSig(
                    trilist.UShortInput[(uint) ( joinMap.OutputVideo.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(2, this, "Linking {0} OutputCurrentVideoInputNames to join {1}", item.DeviceActual.Key, joinMap.OutputCurrentVideoInputNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.CurrentVideoRouteName.LinkInputSig(
                    trilist.StringInput[(uint) ( joinMap.OutputCurrentVideoInputNames.JoinNumber + item.DeviceId - 1 )]);

                Debug.Console(2, this, "Linking {0} OutputEdidManufacturer to join {1}", item.DeviceActual.Key, joinMap.OutputEdidManufacturer.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.EdidManufacturer.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.OutputEdidManufacturer.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} OutputAspectRatioMode to join {1}", item.DeviceActual.Key, joinMap.OutputAspectRatioMode.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.AspectRatioMode.LinkInputSig(
                    trilist.UShortInput[(uint)(joinMap.OutputAspectRatioMode.JoinNumber + item.DeviceId - 1)]);

                var rx = item.DeviceActual;
                var stream = rx.Device as IStreamWithHardware;
                trilist.SetUShortSigAction((uint) ( joinMap.OutputVideo.JoinNumber + item.DeviceId - 1 ),
                    s =>
                        {
                            if (s == 0)
                            {
                                if (stream != null)
                                    stream.ClearStream();

                                rx.Display.ReleaseRoute();
                            }
                            else
                            {
                                NvxApplicationVideoTransmitter device;
                                if (!_transmitters.TryGetValue(s, out device))
                                    return;

                                rx.Display.ReleaseAndMakeRoute(device.Source, _enableAudioBreakaway ? eRoutingSignalType.Video : eRoutingSignalType.AudioVideo);   
                            }
                        });

                var hdmiOut = item.DeviceActual.Device as IVideowallMode;
                if (hdmiOut != null)
                {
                    trilist.SetUShortSigAction((uint) ( joinMap.OutputAspectRatioMode.JoinNumber + item.DeviceId - 1 ), hdmiOut.SetVideoAspectRatioMode);
                }
            }
        }

        private void LinkAudioTransmitters(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _audioTransmitters.Select(x => new { DeviceId = x.Key, DeviceActual = x.Value }))
            {
                Debug.Console(2, this, "Linking {0} Input Audio Name to join {1}", item.DeviceActual.Key, joinMap.InputAudioNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.AudioName.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.InputAudioNames.JoinNumber + item.DeviceId - 1)]);
            }
        }

        private void LinkAudioReceivers(BasicTriList trilist, NvxApplicationJoinMap joinMap)
        {
            foreach (var item in _audioReceivers.Select(x => new { DeviceId = x.Key, DeviceActual = x.Value }))
            {
                Debug.Console(2, this, "Linking {0} Output Audio Name to join {1}", item.DeviceActual.Key, joinMap.OutputAudioNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.AudioName.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.OutputAudioNames.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} Output Audio to join {1}", item.DeviceActual.Key, joinMap.OutputAudio.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.CurrentAudioRouteId.LinkInputSig(
                    trilist.UShortInput[(uint)(joinMap.OutputAudio.JoinNumber + item.DeviceId - 1)]);

                Debug.Console(2, this, "Linking {0} Current Output Audio Name to join {1}", item.DeviceActual.Key, joinMap.OutputCurrentAudioInputNames.JoinNumber + item.DeviceId - 1);
                item.DeviceActual.CurrentAudioRouteName.LinkInputSig(
                    trilist.StringInput[(uint)(joinMap.OutputCurrentAudioInputNames.JoinNumber + item.DeviceId - 1)]);

                var rx = item.DeviceActual;
                var audioStream = rx.Device as ISecondaryAudioStream;

                trilist.SetUShortSigAction((uint)(joinMap.OutputAudio.JoinNumber + item.DeviceId - 1),
                    s =>
                    {
                        if (s == 0)
                        {
                            rx.Device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                            if (audioStream != null)
                                audioStream.ClearSecondaryStream();

                            rx.Amp.ReleaseRoute();
                        }
                        else
                        {
                            NvxApplicationAudioTransmitter device;
                            if (!_audioTransmitters.TryGetValue(s, out device))
                                return;

                            rx.Amp.ReleaseAndMakeRoute(device.Source, eRoutingSignalType.Audio);
                        }
                    });
            }
        }
    }
}