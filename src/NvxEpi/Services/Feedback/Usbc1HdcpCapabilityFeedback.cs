using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc1HdcpCapabilityFeedback
{
    public const string Key = "Usbc1HdcpCapability";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[1] == null)
            return new StringFeedback(() => string.Empty);

        var feedback = new StringFeedback(Key,
            () => device.UsbcIn[1].HdcpCapabilityFeedback.ToString());

        device.UsbcIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class Usbc1HdcpStateFeedback
{
    public const string Key = "Usbc1HdcpState";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[1] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(Key,
            () => (int)device.UsbcIn[1].VideoAttributes.HdcpStateFeedback);

        device.UsbcIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class Usbc1CurrentResolutionFeedback
{
    public const string Key = "Usbc1CurrentResolution";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if(device.UsbcIn == null || device.UsbcIn[1] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () =>
        {
            var resolution = string.Format("{0}x{1}@{2}", device.UsbcIn[1].VideoAttributes.HorizontalResolutionFeedback.UShortValue, device.UsbcIn[1].VideoAttributes.VerticalResolutionFeedback.UShortValue, device.UsbcIn[1].VideoAttributes.FramesPerSecondFeedback.UShortValue);
            return resolution;
        });

        device.UsbcIn[1].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc1AudioChannelsFeedback
{
    public const string Key = "Usbc1AudioChannels";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if(device.UsbcIn == null | device.UsbcIn[1] == null)
        {
            return new IntFeedback(() => 0);
        }

        var feedback = new IntFeedback(Key, () => device.UsbcIn[1].AudioChannelsFeedback.UShortValue);

        device.UsbcIn[1].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc1AudioFormatFeedback
{
    public const string Key = "Usbc1AudioFormat";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[1] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[1].AudioFormatFeedback.ToString());

        device.UsbcIn[1].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc1ColorSpaceFeedback
{
    public const string Key = "Usbc1Colorspace";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[1] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[1].VideoAttributes.ColorSpaceFeedback.ToString());

        device.UsbcIn[1].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc1HdrTypeFeedback
{
    public const string Key = "Usbc1Colorspace";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[1] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[1].HdrTypeFeedback.ToString());

        device.UsbcIn[1].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}