using NvxEpi.Abstractions.Dante;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentDanteInput : IDanteAudio
    {
        StringFeedback CurrentDanteInput { get; }
        IntFeedback CurrentDanteInputValue { get; }
    }
}