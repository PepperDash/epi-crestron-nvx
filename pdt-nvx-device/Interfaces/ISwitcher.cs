using System;
using PepperDash.Essentials.Core;
using EssentialsExtensions;
using EssentialsExtensions.Attributes;

namespace NvxEpi.Interfaces
{
    public interface ISwitcher : IDynamicFeedback
    {
        Feedback Feedback { get; set; }
        event EventHandler RouteUpdated;
        int Source { get; set; }
        string CurrentlyRouted { get; }
    }
}
