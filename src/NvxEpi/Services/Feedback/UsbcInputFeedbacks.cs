using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class UsbcSyncDetectedFeedback
{
    public const string Key = "Usbc{0}SyncDetected";

    public static BoolFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting SyncDetected Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
            return new BoolFeedback(() => false);

        var feedback = new BoolFeedback(string.Format(Key, _inputNumber),
            () => device.UsbcIn[_inputNumber].SyncDetectedFeedback.BoolValue);

        device.UsbcIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcHdcpSupportFeedback
{
    public const string Key = "Usbc{0}HdcpSupport";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting HdcpSupport for input {0}", _inputNumber);

        if(device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, _inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.UsbcIn[_inputNumber].HdcpSupportedLevelFeedback.ToString());

        device.UsbcIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcHdcpCapabilityValueFeedback
{        
    public const string Key = "Usbc{0}HdcpCapabilityValue";

    public static IntFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting HdcpCapabilityValue Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(string.Format(Key, _inputNumber),
            () => (int)device.UsbcIn[_inputNumber].HdcpCapabilityFeedback);

        device.UsbcIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}
public class UsbcHdcpCapabilityFeedback
{
    public const string Key = "Usbc{0}HdcpCapability";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting HdcpCapability Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
            return new StringFeedback(() => string.Empty);

        var feedback = new StringFeedback(string.Format(Key, _inputNumber),
            () => device.UsbcIn[_inputNumber].HdcpCapabilityFeedback.ToString());

        device.UsbcIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcHdcpStateFeedback
{
    public const string Key = "Usbc{0}HdcpState";

    public static IntFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting HdcpState Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(string.Format(Key, _inputNumber),
            () => (int)device.UsbcIn[_inputNumber].VideoAttributes.HdcpStateFeedback);

        
        device.UsbcIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcCurrentResolutionFeedback
{
    public const string Key = "Usbc{0}CurrentResolution";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting CurrentResolution Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, _inputNumber), () =>
        {
            var resolution = string.Format("{0}x{1}@{2}", device.UsbcIn[_inputNumber].VideoAttributes.HorizontalResolutionFeedback.UShortValue, device.UsbcIn[_inputNumber].VideoAttributes.VerticalResolutionFeedback.UShortValue, device.UsbcIn[_inputNumber].VideoAttributes.FramesPerSecondFeedback.UShortValue);
            return resolution;
        });

        device.UsbcIn[_inputNumber].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcAudioChannelsFeedback
{
    public const string Key = "Usbc{0}AudioChannels";

    public static IntFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting AudioResolution Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new IntFeedback(() => 0);
        }

        var feedback = new IntFeedback(string.Format(Key, _inputNumber), () => device.UsbcIn[_inputNumber].AudioChannelsFeedback.UShortValue);

        device.UsbcIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcAudioFormatFeedback
{
    public const string Key = "Usbc{0}AudioFormat";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting AudioFormat Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.UsbcIn[_inputNumber].AudioFormatFeedback.ToString());

        device.UsbcIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcColorSpaceFeedback
{
    public const string Key = "Usbc{0}Colorspace";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting Colorspace Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.UsbcIn[_inputNumber].VideoAttributes.ColorSpaceFeedback.ToString());

        device.UsbcIn[_inputNumber].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcHdrTypeFeedback
{
    public const string Key = "Usbc{0}HdrType";

    public static StringFeedback GetFeedback(DmNvx38x device, uint _inputNumber)
    {
        Debug.LogInformation("Getting HdrType Feedback for {0}", _inputNumber);
        if (device.UsbcIn == null || device.UsbcIn[_inputNumber] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.UsbcIn[_inputNumber].HdrTypeFeedback.ToString());

        device.UsbcIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}
