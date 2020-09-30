using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using NvxEpi.Enums;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Services
{
    public class TieLineConnector
    {
        public static void AddTieLinesForSources(ReadOnlyDictionary<int, DummyRoutingInputsDevice> sources, 
            ReadOnlyDictionary<int, INvxDevice> trasnmitters)
        {
            foreach (var source in sources)
            {
                INvxDevice tx;
                if (!trasnmitters.TryGetValue(source.Key, out tx)) continue;

                var inputPort = tx.InputPorts[DeviceInputEnum.Hdmi1.Name];
                if (inputPort == null)
                    throw new ArgumentNullException("inputPort");

                TieLineCollection.Default.Add(new TieLine(source.Value.AudioVideoOutputPort, inputPort,
                    eRoutingSignalType.AudioVideo));
            }
        }

        public static void AddTieLinesForAudioDestinations(ReadOnlyDictionary<int, Amplifier> audioDestinations,
            ReadOnlyDictionary<int, INvxDevice> receivers)
        {
            foreach (var dest in audioDestinations)
            {
                INvxDevice rx;
                if (!receivers.TryGetValue(dest.Key, out rx)) continue;

                var outputPort = rx.OutputPorts[AnalogAudioOutput.Key];
                if (outputPort == null)
                    continue;

                TieLineCollection.Default.Add(new TieLine(outputPort, dest.Value.AudioIn, eRoutingSignalType.Audio));
            }
        }

        public static void AddTieLinesForVideoDestinations(ReadOnlyDictionary<int, MockDisplay> videoDestinations,
            ReadOnlyDictionary<int, INvxDevice> receivers)
        {
            foreach (var dest in videoDestinations)
            {
                INvxDevice rx;
                if (!receivers.TryGetValue(dest.Key, out rx)) continue;

                var outputPort = rx.OutputPorts[HdmiOutput.Key];
                if (outputPort == null)
                    throw new ArgumentNullException("outputPort");

                TieLineCollection.Default.Add(new TieLine(outputPort, dest.Value.HdmiIn1, eRoutingSignalType.AudioVideo));
            }
        }

        public static void AddTieLinesForTransmitters(IEnumerable<INvxDevice> trasnmitters)
        {
            foreach (var item in trasnmitters)
            {
                var tx = item;
                var outputPort = tx.OutputPorts[StreamOutput.Key];
                if (outputPort == null)
                    throw new NullReferenceException("outputPort");

                var stream = tx as IStream;
                if (tx is ISecondaryAudioStream)
                {
                    var streamInput = NvxGlobalRouter
                        .Instance
                        .PrimaryStreamRouter
                        .InputPorts[PrimaryStreamRouter.GetInputPortKeyForTx(stream)];

                    if (streamInput == null)
                        throw new NullReferenceException("PrimaryRouterStreamInput");

                    TieLineCollection.Default.Add(new TieLine(outputPort, streamInput, eRoutingSignalType.Video));

                    var secondaryAudio = tx as ISecondaryAudioStream;
                    var secondaryAudioPort = tx.OutputPorts[SecondaryAudioOutput.Key];
                    if (secondaryAudioPort == null)
                        throw new NullReferenceException("secondaryAudioOutput");

                    var secondaryAudioInput = NvxGlobalRouter
                        .Instance
                        .SecondaryAudioRouter
                        .InputPorts[SecondaryAudioRouter.GetInputPortKeyForTx(secondaryAudio)];

                    if (secondaryAudioInput == null)
                        throw new NullReferenceException("SecondaryRouterStreamInput");

                    TieLineCollection.Default.Add(new TieLine(secondaryAudioPort, secondaryAudioInput, eRoutingSignalType.Audio));

                    continue;
                }

                TieLineCollection.Default.Add(new TieLine(outputPort, NvxGlobalRouter
                    .Instance
                    .PrimaryStreamRouter
                    .InputPorts[PrimaryStreamRouter.GetInputPortKeyForTx(stream)], eRoutingSignalType.AudioVideo));
            }
        }

        public static void AddTieLinesForReceivers(IEnumerable<INvxDevice> receivers)
        {
            foreach (var item in receivers)
            {
                var rx = item;
                var inputPort = rx.InputPorts[DeviceInputEnum.Stream.Name];
                if (inputPort == null)
                    throw new NullReferenceException("inputPort");

                var stream = rx as IStream;
                if (rx is ISecondaryAudioStream)
                {
                    var streamOutput = NvxGlobalRouter
                        .Instance
                        .PrimaryStreamRouter
                        .OutputPorts[PrimaryStreamRouter.GetOutputPortKeyForRx(stream)];

                    if (streamOutput == null)
                        throw new NullReferenceException("PrimaryRouterStreamOutput");

                    TieLineCollection.Default.Add(new TieLine(streamOutput, inputPort, eRoutingSignalType.Video));

                    var secondaryAudio = rx as ISecondaryAudioStream;
                    var secondaryAudioPort = rx.InputPorts[DeviceInputEnum.SecondaryAudio.Name];
                    if (secondaryAudioPort == null)
                        throw new NullReferenceException("SecondaryRouterInput");

                    var secondaryAudioStreamOutput = NvxGlobalRouter
                        .Instance
                        .SecondaryAudioRouter
                        .OutputPorts[SecondaryAudioRouter.GetOutputPortKeyForRx(secondaryAudio)];

                    if (secondaryAudioStreamOutput == null)
                        throw new NullReferenceException("SecondaryRouterStreamInput");

                    TieLineCollection.Default.Add(new TieLine(secondaryAudioStreamOutput, secondaryAudioPort,
                        eRoutingSignalType.Audio));

                    continue;
                }

                TieLineCollection.Default.Add(new TieLine(NvxGlobalRouter
                    .Instance
                    .PrimaryStreamRouter
                    .OutputPorts[PrimaryStreamRouter.GetOutputPortKeyForRx(stream)], inputPort, eRoutingSignalType.AudioVideo));
            }
        }
    }
}