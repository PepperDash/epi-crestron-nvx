﻿using Crestron.SimplSharp;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.DmInput;

public interface IDmInput : INvxDeviceWithHardware
{
    ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; }
    ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; }
    /*
    ReadOnlyDictionary<uint, StringFeedback> CurrentResolution { get; }

    ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString { get; }

    ReadOnlyDictionary<uint, StringFeedback> HdcpSupport { get; }
    ReadOnlyDictionary<uint, IntFeedback> AudioChannels { get; }

    ReadOnlyDictionary<uint, StringFeedback> AudioFormat { get; }

    ReadOnlyDictionary<uint, StringFeedback> ColorSpace { get; }

    ReadOnlyDictionary<uint, StringFeedback> HdrType { get; }
    */
}