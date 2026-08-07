using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.InputSwitching;

public class VideoInputSwitcher : ICurrentVideoInput
{
    private readonly StringFeedback _currentVideoInput;
    private readonly IntFeedback _currentVideoInputValue;
    private readonly BoolFeedback _automaticInputRoutingEnabled;
    private readonly INvxDeviceWithHardware _device;

    public VideoInputSwitcher(INvxDeviceWithHardware device)
    {
        _device = device;

        _currentVideoInput = VideoInputFeedback.GetFeedback(Hardware);
        _currentVideoInputValue = VideoInputValueFeedback.GetFeedback(Hardware);
        _automaticInputRoutingEnabled = AutomaticInputRoutingEnabledFeedback.GetFeedback(Hardware);

        Feedbacks.Add(_currentVideoInput);
        Feedbacks.Add(_currentVideoInputValue);
        Feedbacks.Add(_automaticInputRoutingEnabled);
    }

    public StringFeedback CurrentVideoInput
    {
        get { return _currentVideoInput; }
    }

    public IntFeedback CurrentVideoInputValue
    {
        get { return _currentVideoInputValue; }
    }

    public BoolFeedback AutomaticInputRoutingEnabled
    {
        get { return _automaticInputRoutingEnabled; }
    }

    public int DeviceId
    {
        get { return _device.DeviceId; }
    }

    public IntFeedback DeviceMode
    {
        get { return _device.DeviceMode; }
    }

    public FeedbackCollection<Feedback> Feedbacks
    {
        get { return _device.Feedbacks; }
    }

    public DmNvxBaseClass Hardware
    {
        get { return _device.Hardware; }
    }

    public RoutingPortCollection<RoutingInputPort> InputPorts
    {
        get { return _device.InputPorts; }
    }

    public BoolFeedback IsOnline
    {
        get { return _device.IsOnline; }
    }

    public bool IsTransmitter
    {
        get { return _device.IsTransmitter; }
    }

    public string Key
    {
        get { return _device.Key; }
    }

    public string Name
    {
        get { return _device.Name; }
    }

    public RoutingPortCollection<RoutingOutputPort> OutputPorts
    {
        get { return _device.OutputPorts; }
    }

    public bool IsEnabled => _device.IsEnabled;

    public BoolFeedback EnabledFeedback => _device.EnabledFeedback;
}