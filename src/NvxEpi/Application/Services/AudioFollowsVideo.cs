using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Entities.Routing;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Services
{
    public class AudioFollowsVideoHandler
    {
        private readonly Dictionary<int, INvxDevice> _transmitters;
        private readonly Dictionary<int, INvxDevice> _receivers;
        private readonly Dictionary<int, INvxDevice> _audioTransmitters;
        private readonly Dictionary<int, INvxDevice> _audioReceivers;

        public AudioFollowsVideoHandler(Dictionary<int, INvxDevice> transmitters, Dictionary<int, INvxDevice> receivers, Dictionary<int, INvxDevice> audioTransmitters, Dictionary<int, INvxDevice> audioReceivers)
        {
            _transmitters = transmitters;
            _receivers = receivers;
            _audioTransmitters = audioTransmitters;
            _audioReceivers = audioReceivers;
        }

        public void SetAudioFollowsVideoTrue()
        {
            foreach (var transmitter in _transmitters)
            {
                var tx = transmitter.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.PrimaryStreamRouter.Key))
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(tx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.AudioVideo;
                }
            }

            foreach (var receiver in _receivers)
            {
                var rx = receiver.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.PrimaryStreamRouter.Key))
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(rx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.AudioVideo;
                }
            }
        }

        public void SetAudioFollowsVideoFalse()
        {
            foreach (var transmitter in _transmitters)
            {
                var tx = transmitter.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.PrimaryStreamRouter.Key))
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(tx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.Video;
                }
            }

            foreach (var receiver in _receivers)
            {
                var rx = receiver.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.PrimaryStreamRouter.Key))
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(rx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.Video;
                }
            }

            foreach (var transmitter in _audioTransmitters)
            {
                var tx = transmitter.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.SecondaryAudioRouter.Key))
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(tx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.Audio;
                }
            }

            foreach (var receiver in _audioReceivers)
            {
                var rx = receiver.Value;

                foreach (var tieLine in TieLineCollection
                    .Default
                    .Where(tieLine => tieLine.SourcePort.ParentDevice.Key.Equals(NvxGlobalRouter.Instance.SecondaryAudioRouter.Key))
                    .Where(tieLine => tieLine.DestinationPort.ParentDevice.Key.Equals(rx.Key)))
                {
                    tieLine.OverrideType = eRoutingSignalType.Audio;
                }
            }
        }
    }
}