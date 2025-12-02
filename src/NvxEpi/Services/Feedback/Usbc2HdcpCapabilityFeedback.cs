using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc2HdcpCapabilityFeedback
{
    public const string Key = "Usbc2HdcpCapability";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[2] == null)
            return new StringFeedback(() => string.Empty);

        var feedback = new StringFeedback(Key,
            () => device.UsbcIn[2].HdcpCapabilityFeedback.ToString());

        device.UsbcIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class Usbc2HdcpStateFeedback
{
    public const string Key = "Usbc2HdcpState";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[2] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(Key,
            () => (int)device.UsbcIn[2].VideoAttributes.HdcpStateFeedback);

        device.UsbcIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class Usbc2CurrentResolutionFeedback
{
    public const string Key = "Usbc2CurrentResolution";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if(device.UsbcIn == null || device.UsbcIn[2] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () =>
        {
            var resolution = string.Format("{0}x{1}@{2}", device.UsbcIn[2].VideoAttributes.HorizontalResolutionFeedback.UShortValue, device.UsbcIn[2].VideoAttributes.VerticalResolutionFeedback.UShortValue, device.UsbcIn[2].VideoAttributes.FramesPerSecondFeedback.UShortValue);
            return resolution;
        });

        device.UsbcIn[2].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc2AudioChannelsFeedback
{
    public const string Key = "Usbc2AudioChannels";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if(device.UsbcIn == null | device.UsbcIn[2] == null)
        {
            return new IntFeedback(() => 0);
        }

        var feedback = new IntFeedback(Key, () => device.UsbcIn[2].AudioChannelsFeedback.UShortValue);

        device.UsbcIn[2].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc2AudioFormatFeedback
{
    public const string Key = "Usbc2AudioFormat";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[2] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[2].AudioFormatFeedback.ToString());

        device.UsbcIn[2].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc2ColorSpaceFeedback
{
    public const string Key = "Usbc2Colorspace";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[2] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[2].VideoAttributes.ColorSpaceFeedback.ToString());

        device.UsbcIn[2].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}

public class Usbc2HdrTypeFeedback
{
    public const string Key = "Usbc2Colorspace";

    public static StringFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null | device.UsbcIn[2] == null)
        {
            return new StringFeedback(() => string.Empty);
        }

        var feedback = new StringFeedback(Key, () => device.UsbcIn[2].HdrTypeFeedback.ToString());

        device.UsbcIn[2].StreamChange += (s, a) => feedback.FireUpdate();

        return feedback;
    }
}