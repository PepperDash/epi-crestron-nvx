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
    public class ApplicationTieLineConnector
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
    }
}