using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasHasNaxAudioStreamRouting : IHasNaxAudioTxStream, IHasNaxAudioRxStream
    {
        IntFeedback CurrentNaxAudioRouteValue { get; }
        StringFeedback CurrentNaxAudioRouteName { get; }
    }
}