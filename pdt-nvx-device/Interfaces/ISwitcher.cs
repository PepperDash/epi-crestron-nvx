using System;
using System.Collections.Generic;
using PepperDash.Essentials.Core;

namespace NvxEpi.Interfaces
{
    public interface ISwitcher
    {
        Feedback Feedback { get; set; }
        event EventHandler RouteUpdated;
        int Source { get; set; }
        string CurrentlyRouted { get; }

        void SetInputs(IEnumerable<INvxDevice> inputs);
    }
}
