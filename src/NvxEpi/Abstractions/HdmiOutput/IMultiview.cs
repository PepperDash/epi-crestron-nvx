using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput;

public interface IMultiview : IHdmiOutput
{
    BoolFeedback MultiviewEnabled { get; }
    IntFeedback MultiviewLayout { get; }
    StringFeedback WindowAStreamUrl { get; }
    StringFeedback WindowBStreamUrl { get; }
    StringFeedback WindowCStreamUrl { get; }
    StringFeedback WindowDStreamUrl { get; }
    StringFeedback WindowEStreamUrl { get; }
    StringFeedback WindowFStreamUrl { get; }
}