using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Features.Routing;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi.Services.TieLines;

public class TieLineConnector
{
    public static void AddTieLinesForTransmitters(IEnumerable<INvxDevice> transmitters)
    {
        foreach (var item in transmitters)
        {
            item.LogVerbose("Generating tx tieLine");
            var tx = item;
            var outputPort = tx.OutputPorts[SwitcherForStreamOutput.Key] ?? throw new NullReferenceException("outputPort");
            var stream = tx as IStream;

            var streamInput = NvxGlobalRouter
                .Instance
                .PrimaryStreamRouter
                .InputPorts[PrimaryStreamRouter.GetInputPortKeyForTx(stream)] ?? throw new NullReferenceException("PrimaryRouterStreamInput");

            var tieLine = new TieLine(outputPort, streamInput, eRoutingSignalType.AudioVideo);

            item.LogVerbose("Adding tx {tieLine}", tieLine);

            TieLineCollection.Default.Add(tieLine);
        }
    }

    public static void AddTieLinesForReceivers(IEnumerable<INvxDevice> receivers)
    {
        foreach (var item in receivers)
        {
            item.LogVerbose("Generating rx tieLine");
            var rx = item;
            var inputPort = rx.InputPorts[DeviceInputEnum.Stream.Name] ?? throw new NullReferenceException("inputPort");
            var stream = rx as IStream;

            var streamOutput = NvxGlobalRouter
                .Instance
                .PrimaryStreamRouter
                .OutputPorts[PrimaryStreamRouter.GetOutputPortKeyForRx(stream)] ?? throw new NullReferenceException("PrimaryRouterStreamOutput");

            var tieLine = new TieLine(streamOutput, inputPort, eRoutingSignalType.AudioVideo);

            item.LogVerbose("Adding rx {tieLine}", tieLine);

            TieLineCollection.Default.Add(tieLine);
        }
    }

    public static void AddTieLinesForAudioTransmitters(IEnumerable<INvxDevice> transmitters)
    {
        foreach (var secondaryAudio in transmitters.OfType<ISecondaryAudioStream>())
        {
            secondaryAudio.LogVerbose("Generating secondaryAudio tx TieLine");

            var secondaryAudioPort = secondaryAudio.OutputPorts[SwitcherForSecondaryAudioOutput.Key] ?? throw new NullReferenceException("secondaryAudioInput");
            var secondaryAudioInput = NvxGlobalRouter
                .Instance
                .SecondaryAudioRouter
                .InputPorts[SecondaryAudioRouter.GetInputPortKeyForTx(secondaryAudio)] ?? throw new NullReferenceException("SecondaryAudioStreamInput");

            var tieLine = new TieLine(secondaryAudioPort, secondaryAudioInput, eRoutingSignalType.Audio);

            secondaryAudio.LogVerbose("Adding secondaryAudio tx {tieLine}", tieLine);

            TieLineCollection.Default.Add(tieLine);
        }
    }

    public static void AddTieLinesForAudioReceivers(IEnumerable<INvxDevice> receivers)
    {
        foreach (var secondaryAudio in receivers.OfType<ISecondaryAudioStream>())
        {
            secondaryAudio.LogVerbose("Generating secondaryAudio rx TieLine");

            var secondaryAudioPort = secondaryAudio.InputPorts[DeviceInputEnum.SecondaryAudio.Name] ?? throw new NullReferenceException("SecondaryRouterInput");
            var secondaryAudioStreamOutput = NvxGlobalRouter
                .Instance
                .SecondaryAudioRouter
                .OutputPorts[SecondaryAudioRouter.GetOutputPortKeyForRx(secondaryAudio)] ?? throw new NullReferenceException("SecondaryRouterStreamInput");

            var tieLine = new TieLine(secondaryAudioStreamOutput, secondaryAudioPort, eRoutingSignalType.Audio);

            secondaryAudio.LogVerbose("Adding secondaryAudio rx {tieLine}", tieLine);

            TieLineCollection.Default.Add(tieLine);
        }
    }
}