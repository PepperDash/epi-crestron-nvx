using System;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Services
{
    public class ApplicationTieLineConnector
    {
        public static void AddTieLineForDummySource(DummyRoutingInputsDevice source, INvxDevice tx)
        {
            var inputPort = tx.InputPorts[DeviceInputEnum.Hdmi1.Name];
            if (inputPort == null)
                throw new ArgumentNullException("inputPort");

            TieLineCollection.Default.Add(new TieLine(source.AudioVideoOutputPort, inputPort, eRoutingSignalType.AudioVideo));
        }

        public static void AddTieLineForAmp(Amplifier amp, INvxDevice rx)
        {
            var outputPort = rx.OutputPorts[SwitcherForAnalogAudioOutput.Key];
            if (outputPort == null)
                return;

            TieLineCollection.Default.Add(new TieLine(outputPort, amp.AudioIn, eRoutingSignalType.Audio));
        }

        public static void AddTieLineForMockDisplay(MockDisplay dest, INvxDevice rx)
        {
            var outputPort = rx.OutputPorts[SwitcherForHdmiOutput.Key];
            if (outputPort == null)
                throw new ArgumentNullException("outputPort");

            TieLineCollection.Default.Add(new TieLine(outputPort, dest.HdmiIn1, eRoutingSignalType.AudioVideo));
        }
    }
}