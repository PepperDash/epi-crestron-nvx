using System;
using System.Collections.Generic;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using NvxEpi.Enums;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.TieLines
{
    public class TieLineConnector
    {
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