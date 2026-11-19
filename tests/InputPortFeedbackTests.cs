using FluentAssertions;
using Xunit;

namespace NvxEpi.Tests;

// Every RoutingInputPortWithVideoStatuses builds its VideoStatus feedbacks under the same
// hardcoded keys (HasVideoStatusFeedback, VideoSyncFeedback, ...). Only one input port per
// device may add them to device.Feedbacks; a second Add throws ArgumentException, which aborts
// Nvx38X.AddRoutingPorts() and leaves every later port (including the encoder stream output)
// unregistered. Source scanning keeps that from coming back.
public class InputPortFeedbackTests
{
    [Theory]
    [InlineData("HdmiInput2Port")]
    [InlineData("UsbcInput1Port")]
    [InlineData("UsbcInput2Port")]
    public void Secondary_Input_Port_Does_Not_Register_VideoStatus_Feedbacks(string portClassName)
    {
        var source = AssemblyFixture.FindSourceForClass(portClassName);
        source.Should().NotBeNull($"source for '{portClassName}' should exist");
        source!.Should().NotContain("device.Feedbacks.Add(videoStatusOutput)",
            $"{portClassName} must not re-register the VideoStatus feedbacks HdmiInput1Port already added");
    }
}
