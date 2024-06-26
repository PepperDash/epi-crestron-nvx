using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Features.Routing;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.TieLines;

public class TieLineConnector
{
    public static void AddTieLinesForTransmitters(IEnumerable<INvxDevice> transmitters)
    {
        foreach (var item in transmitters)
        {
            var tx = item;
            var outputPort = tx.OutputPorts[SwitcherForStreamOutput.Key] ?? throw new NullReferenceException("outputPort");
            var stream = tx as IStream;

            var streamInput = NvxGlobalRouter
                .Instance
                .PrimaryStreamRouter
                .InputPorts[PrimaryStreamRouter.GetInputPortKeyForTx(stream)] ?? throw new NullReferenceException("PrimaryRouterStreamInput");
            TieLineCollection.Default.Add(new TieLine(outputPort, streamInput, eRoutingSignalType.AudioVideo));
        }
    }

    public static void AddTieLinesForReceivers(IEnumerable<INvxDevice> receivers)
    {
        foreach (var item in receivers)
        {
            var rx = item;
            var inputPort = rx.InputPorts[DeviceInputEnum.Stream.Name] ?? throw new NullReferenceException("inputPort");
            var stream = rx as IStream;

            var streamOutput = NvxGlobalRouter
                .Instance
                .PrimaryStreamRouter
                .OutputPorts[PrimaryStreamRouter.GetOutputPortKeyForRx(stream)] ?? throw new NullReferenceException("PrimaryRouterStreamOutput");
            TieLineCollection.Default.Add(new TieLine(streamOutput, inputPort, eRoutingSignalType.AudioVideo));
        }
    }

    public static void AddTieLinesForAudioTransmitters(IEnumerable<INvxDevice> transmitters)
    {
        foreach (var secondaryAudio in transmitters.OfType<ISecondaryAudioStream>())
        {
            var secondaryAudioPort = secondaryAudio.OutputPorts[SwitcherForSecondaryAudioOutput.Key] ?? throw new NullReferenceException("secondaryAudioInput");
            var secondaryAudioInput = NvxGlobalRouter
                .Instance
                .SecondaryAudioRouter
                .InputPorts[SecondaryAudioRouter.GetInputPortKeyForTx(secondaryAudio)] ?? throw new NullReferenceException("SecondaryAudioStreamInput");
            TieLineCollection.Default.Add(new TieLine(secondaryAudioPort, secondaryAudioInput, eRoutingSignalType.Audio));
        }
    }

    public static void AddTieLinesForAudioReceivers(IEnumerable<INvxDevice> receivers)
    {
        foreach (var secondaryAudio in receivers.OfType<ISecondaryAudioStream>())
        {
            var secondaryAudioPort = secondaryAudio.InputPorts[DeviceInputEnum.SecondaryAudio.Name] ?? throw new NullReferenceException("SecondaryRouterInput");
            var secondaryAudioStreamOutput = NvxGlobalRouter
                .Instance
                .SecondaryAudioRouter
                .OutputPorts[SecondaryAudioRouter.GetOutputPortKeyForRx(secondaryAudio)] ?? throw new NullReferenceException("SecondaryRouterStreamInput");
            TieLineCollection.Default.Add(new TieLine(secondaryAudioStreamOutput, secondaryAudioPort, eRoutingSignalType.Audio));
        }
    }
}