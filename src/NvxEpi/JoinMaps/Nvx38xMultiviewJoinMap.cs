using PepperDash.Essentials.Core;
using System;

namespace NvxEpi.JoinMaps;

/// <summary>
/// JoinMap for DM-NVX-38x devices with multiview support
/// Extends the base NvxDeviceJoinMap with additional multiview-specific joins
/// </summary>
public class Nvx38xMultiviewJoinMap : NvxDeviceJoinMap
{
    #region Multiview Control Joins

    [JoinName("MultiviewEnter")]
    public JoinDataComplete MultiviewEnter = new(
        new JoinData
        {
            JoinNumber = 21, // Starting multiview joins at 21
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Digital,
            Description = "Multiview Enter (Sets layouts and window routes. Hold to set immediately.)"
        });

    [JoinName("MultiviewEnabled")]
    public JoinDataComplete MultiviewEnabled = new(
        new JoinData
        {
            JoinNumber = 22,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
            JoinType = eJoinType.Digital,
            Description = "Multiview Enable Set/Feedback (Enables 3rd multicast stream. Requires reboot once set.)"
        });

    [JoinName("MultiviewDisabled")]
    public JoinDataComplete MultiviewDisabled = new(
        new JoinData
        {
            JoinNumber = 23,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
            JoinType = eJoinType.Digital,
            Description = "Multiview Disabled Set/Feedback (Removes 3rd multicast stream. Requires reboot once set.)"
        });

    [JoinName("MultiviewLayout")]
    public JoinDataComplete MultiviewLayout = new(
        new JoinData
        {
            JoinNumber = 21,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Analog,
            Description = "Multiview Layout Set (1-18)"
        });

    [JoinName("MultiviewLayoutFeedback")]
    public JoinDataComplete MultiviewLayoutFeedback = new(
        new JoinData
        {
            JoinNumber = 21,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Analog,
            Description = "Multiview Layout Feedback (1-18)"
        });

    [JoinName("MultiviewAudioSource")]
    public JoinDataComplete MultiviewAudioSource = new(
        new JoinData
        {
            JoinNumber = 22,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Analog,
            Description = "Multiview Audio Source Selection"
        });

    [JoinName("MultiviewAudioSourceFeedback")]
    public JoinDataComplete MultiviewAudioSourceFeedback = new(
        new JoinData
        {
            JoinNumber = 22,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Analog,
            Description = "Multiview Audio Source Feedback"
        });

    #endregion

    #region Window Stream URL Joins

    [JoinName("Window1StreamUrl")]
    public JoinDataComplete Window1StreamUrl = new(
        new JoinData
        {
            JoinNumber = 21,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 1 Stream URL"
        });

    [JoinName("Window2StreamUrl")]
    public JoinDataComplete Window2StreamUrl = new(
        new JoinData
        {
            JoinNumber = 22,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 2 Stream URL"
        });

    [JoinName("Window3StreamUrl")]
    public JoinDataComplete Window3StreamUrl = new(
        new JoinData
        {
            JoinNumber = 23,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 3 Stream URL"
        });

    [JoinName("Window4StreamUrl")]
    public JoinDataComplete Window4StreamUrl = new(
        new JoinData
        {
            JoinNumber = 24,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 4 Stream URL"
        });

    [JoinName("Window5StreamUrl")]
    public JoinDataComplete Window5StreamUrl = new(
        new JoinData
        {
            JoinNumber = 25,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 5 Stream URL"
        });

    [JoinName("Window6StreamUrl")]
    public JoinDataComplete Window6StreamUrl = new(
        new JoinData
        {
            JoinNumber = 26,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 6 Stream URL"
        });

    #endregion

    #region Window Stream URL Feedback Joins

    [JoinName("Window1StreamUrlFeedback")]
    public JoinDataComplete Window1StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 21,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 1 Stream URL Feedback"
        });

    [JoinName("Window2StreamUrlFeedback")]
    public JoinDataComplete Window2StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 22,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 2 Stream URL Feedback"
        });

    [JoinName("Window3StreamUrlFeedback")]
    public JoinDataComplete Window3StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 23,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 3 Stream URL Feedback"
        });

    [JoinName("Window4StreamUrlFeedback")]
    public JoinDataComplete Window4StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 24,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 4 Stream URL Feedback"
        });

    [JoinName("Window5StreamUrlFeedback")]
    public JoinDataComplete Window5StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 25,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 5 Stream URL Feedback"
        });

    [JoinName("Window6StreamUrlFeedback")]
    public JoinDataComplete Window6StreamUrlFeedback = new(
        new JoinData
        {
            JoinNumber = 26,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 6 Stream URL Feedback"
        });

    #endregion

    #region Window Label Joins

    [JoinName("Window1Label")]
    public JoinDataComplete Window1Label = new(
        new JoinData
        {
            JoinNumber = 31,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 1 Label"
        });

    [JoinName("Window2Label")]
    public JoinDataComplete Window2Label = new(
        new JoinData
        {
            JoinNumber = 32,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 2 Label"
        });

    [JoinName("Window3Label")]
    public JoinDataComplete Window3Label = new(
        new JoinData
        {
            JoinNumber = 33,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 3 Label"
        });

    [JoinName("Window4Label")]
    public JoinDataComplete Window4Label = new(
        new JoinData
        {
            JoinNumber = 34,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 4 Label"
        });

    [JoinName("Window5Label")]
    public JoinDataComplete Window5Label = new(
        new JoinData
        {
            JoinNumber = 35,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 5 Label"
        });

    [JoinName("Window6Label")]
    public JoinDataComplete Window6Label = new(
        new JoinData
        {
            JoinNumber = 36,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.FromSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 6 Label"
        });

    #endregion

    #region Window Label Feedback Joins

    [JoinName("Window1LabelFeedback")]
    public JoinDataComplete Window1LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 31,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 1 Label Feedback"
        });

    [JoinName("Window2LabelFeedback")]
    public JoinDataComplete Window2LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 32,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 2 Label Feedback"
        });

    [JoinName("Window3LabelFeedback")]
    public JoinDataComplete Window3LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 33,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 3 Label Feedback"
        });

    [JoinName("Window4LabelFeedback")]
    public JoinDataComplete Window4LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 34,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 4 Label Feedback"
        });

    [JoinName("Window5LabelFeedback")]
    public JoinDataComplete Window5LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 35,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 5 Label Feedback"
        });

    [JoinName("Window6LabelFeedback")]
    public JoinDataComplete Window6LabelFeedback = new(
        new JoinData
        {
            JoinNumber = 36,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Window 6 Label Feedback"
        });

    #endregion

    /// <summary>
    /// Constructor for Nvx38xMultiviewJoinMap
    /// </summary>
    /// <param name="joinStart">Starting join number for the device</param>
    public Nvx38xMultiviewJoinMap(uint joinStart)
        : base(joinStart)
    {
        // The multiview joins are defined with absolute join numbers
        // but will be offset by the joinStart value in the base class
    }

    /// <summary>
    /// Helper method to get window stream URL join by window number
    /// </summary>
    /// <param name="windowNumber">Window number (1-6)</param>
    /// <returns>JoinDataComplete for the window stream URL</returns>
    public JoinDataComplete GetWindowStreamUrlJoin(uint windowNumber)
    {
        return windowNumber switch
        {
            1 => Window1StreamUrl,
            2 => Window2StreamUrl,
            3 => Window3StreamUrl,
            4 => Window4StreamUrl,
            5 => Window5StreamUrl,
            6 => Window6StreamUrl,
            _ => throw new ArgumentOutOfRangeException(nameof(windowNumber), "Window number must be between 1-6")
        };
    }

    /// <summary>
    /// Helper method to get window stream URL feedback join by window number
    /// </summary>
    /// <param name="windowNumber">Window number (1-6)</param>
    /// <returns>JoinDataComplete for the window stream URL feedback</returns>
    public JoinDataComplete GetWindowStreamUrlFeedbackJoin(uint windowNumber)
    {
        return windowNumber switch
        {
            1 => Window1StreamUrlFeedback,
            2 => Window2StreamUrlFeedback,
            3 => Window3StreamUrlFeedback,
            4 => Window4StreamUrlFeedback,
            5 => Window5StreamUrlFeedback,
            6 => Window6StreamUrlFeedback,
            _ => throw new ArgumentOutOfRangeException(nameof(windowNumber), "Window number must be between 1-6")
        };
    }

    /// <summary>
    /// Helper method to get window label join by window number
    /// </summary>
    /// <param name="windowNumber">Window number (1-6)</param>
    /// <returns>JoinDataComplete for the window label</returns>
    public JoinDataComplete GetWindowLabelJoin(uint windowNumber)
    {
        return windowNumber switch
        {
            1 => Window1Label,
            2 => Window2Label,
            3 => Window3Label,
            4 => Window4Label,
            5 => Window5Label,
            6 => Window6Label,
            _ => throw new ArgumentOutOfRangeException(nameof(windowNumber), "Window number must be between 1-6")
        };
    }

    /// <summary>
    /// Helper method to get window label feedback join by window number
    /// </summary>
    /// <param name="windowNumber">Window number (1-6)</param>
    /// <returns>JoinDataComplete for the window label feedback</returns>
    public JoinDataComplete GetWindowLabelFeedbackJoin(uint windowNumber)
    {
        return windowNumber switch
        {
            1 => Window1LabelFeedback,
            2 => Window2LabelFeedback,
            3 => Window3LabelFeedback,
            4 => Window4LabelFeedback,
            5 => Window5LabelFeedback,
            6 => Window6LabelFeedback,
            _ => throw new ArgumentOutOfRangeException(nameof(windowNumber), "Window number must be between 1-6")
        };
    }
}