﻿using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IVideowallMode : IHdmiOutput
    {
        IntFeedback VideowallMode { get; }
        void SetVideowallMode(ushort value);
    }
}