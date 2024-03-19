using System;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using MockDisplay = PepperDash.Essentials.Devices.Common.Displays.MockDisplay;

namespace NvxEpi.Application.Services
{
    public class ApplicationTieLineConnector
    {
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
#if SERIES4
            TieLineCollection.Default.Add(new TieLine(outputPort, dest.InputPorts[RoutingPortNames.HdmiIn1], eRoutingSignalType.AudioVideo));
#else
            TieLineCollection.Default.Add(new TieLine(outputPort, dest.HdmiIn1, eRoutingSignalType.AudioVideo));
#endif
        }
    }
}