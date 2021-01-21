﻿using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentVideoInput : INvxHardware
    {
        StringFeedback CurrentVideoInput { get; }
        IntFeedback CurrentVideoInputValue { get; }
    }
}