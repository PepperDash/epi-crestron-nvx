using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class UsbcSyncDetectedFeedback
{
    public const string Key = "Usbc{0}SyncDetected";

    public static BoolFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
            return new BoolFeedback(string.Format(Key, inputNumber), () => false);

        var feedback = new BoolFeedback(string.Format(Key, inputNumber),
            () => nvx38x.UsbcIn[inputNumber].SyncDetectedFeedback.BoolValue);

        nvx38x.UsbcIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcHdcpSupportFeedback
{
    public const string Key = "Usbc{0}HdcpSupport";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, inputNumber), () => nvx38x.UsbcIn[inputNumber].HdcpSupportedLevelFeedback.ToString());

        nvx38x.UsbcIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcHdcpCapabilityValueFeedback
{
    public const string Key = "Usbc{0}HdcpCapabilityValue";

    public static IntFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
            return new IntFeedback(string.Format(Key, inputNumber), () => 0);

        var feedback = new IntFeedback(string.Format(Key, inputNumber),
            () => (int)nvx38x.UsbcIn[inputNumber].HdcpCapabilityFeedback);

        nvx38x.UsbcIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcHdcpCapabilityFeedback
{
    public const string Key = "Usbc{0}HdcpCapability";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);

        var feedback = new StringFeedback(string.Format(Key, inputNumber),
            () => nvx38x.UsbcIn[inputNumber].HdcpCapabilityFeedback.ToString());

        nvx38x.UsbcIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class UsbcCurrentResolutionFeedback
{
    public const string Key = "Usbc{0}CurrentResolution";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, inputNumber), () =>
        {
            var resolution = string.Format("{0}x{1}@{2}",
                nvx38x.UsbcIn[inputNumber].VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                nvx38x.UsbcIn[inputNumber].VideoAttributes.VerticalResolutionFeedback.UShortValue,
                nvx38x.UsbcIn[inputNumber].VideoAttributes.FramesPerSecondFeedback.UShortValue);
            return resolution;
        });

        nvx38x.UsbcIn[inputNumber].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcAudioChannelsFeedback
{
    public const string Key = "Usbc{0}AudioChannels";

    public static IntFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new IntFeedback(string.Format(Key, inputNumber), () => 0);
        }

        var feedback = new IntFeedback(string.Format(Key, inputNumber), () => nvx38x.UsbcIn[inputNumber].AudioChannelsFeedback.UShortValue);

        nvx38x.UsbcIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcAudioFormatFeedback
{
    public const string Key = "Usbc{0}AudioFormat";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, inputNumber), () => nvx38x.UsbcIn[inputNumber].AudioFormatFeedback.ToString());

        nvx38x.UsbcIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcColorSpaceFeedback
{
    public const string Key = "Usbc{0}Colorspace";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, inputNumber), () => nvx38x.UsbcIn[inputNumber].VideoAttributes.ColorSpaceFeedback.ToString());

        nvx38x.UsbcIn[inputNumber].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class UsbcHdrTypeFeedback
{
    public const string Key = "Usbc{0}HdrType";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
    {
        if (device is not DmNvx38x nvx38x || nvx38x.UsbcIn == null || nvx38x.UsbcIn[inputNumber] == null)
        {
            return new StringFeedback(string.Format(Key, inputNumber), () => string.Empty);
        }

        var feedback = new StringFeedback(string.Format(Key, inputNumber), () => nvx38x.UsbcIn[inputNumber].HdrTypeFeedback.ToString());

        nvx38x.UsbcIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}
