using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NvxEpi.Features.Config
{
    /// <summary>
    /// Represents the "properties" property of a device config
    /// 
    /// Example JSON:
    ///  {
    ///     "key": "nvx-rx-display-1",
    ///     "uid": 51,
    ///     "name": "RX Display 1",
    ///     "type": "dmNvx384",
    ///     "group": "dm",
    ///     "properties": {
    ///       "control": {
    ///         "ipId": "51"
    ///       },
    ///       "screens": {
    ///         "1": {
    ///           "enabled": true,
    ///           "name": "Main Screen",
    ///           "screenIndex": 1,
    ///           "layouts": {
    ///             "1": {
    ///               "layoutName": "Full Screen",
    ///               "layoutIndex": 0,
    ///               "layoutType": "fullScreen",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Full Screen",
    ///                   "input": "input1"
    ///                 }
    ///               }
    ///             },
    ///             "2": {
    ///               "layoutName": "Side By Side",
    ///               "layoutIndex": 201,
    ///               "layoutType": "sideBySide",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Left Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Right Window",
    ///                   "input": "input2"
    ///                 }
    ///               }
    ///             },
    ///             "3": {
    ///               "layoutName": "PIP Small Top Left",
    ///               "layoutIndex": 202,
    ///               "layoutType": "pipSmallTopLeft",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Main Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "PIP Window",
    ///                   "input": "input2"
    ///                 }
    ///               }
    ///             },
    ///             "4": {
    ///               "layoutName": "PIP Small Top Right",
    ///               "layoutIndex": 203,
    ///               "layoutType": "pipSmallTopRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Main Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "PIP Window",
    ///                   "input": "input2"
    ///                 }
    ///               }
    ///             },
    ///             "5": {
    ///               "layoutName": "PIP Small Bottom Left",
    ///               "layoutIndex": 204,
    ///               "layoutType": "pipSmallBottomLeft",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Main Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "PIP Window",
    ///                   "input": "input2"
    ///                 }
    ///               }
    ///             },
    ///             "6": {
    ///               "layoutName": "PIP Small Bottom Right",
    ///               "layoutIndex": 205,
    ///               "layoutType": "pipSmallBottomRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Main Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "PIP Window",
    ///                   "input": "input2"
    ///                 }
    ///               }
    ///             },
    ///             "7": {
    ///               "layoutName": "1 Top, 2 Bottom",
    ///               "layoutIndex": 301,
    ///               "layoutType": "oneTopTwoBottom",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Top Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Bottom Left",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Bottom Right",
    ///                   "input": "input3"
    ///                 }
    ///               }
    ///             },
    ///             "8": {
    ///               "layoutName": "2 Top, 1 Bottom",
    ///               "layoutIndex": 302,
    ///               "layoutType": "twoTopOneBottom",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Top Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Top Right",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Bottom Window",
    ///                   "input": "input3"
    ///                 }
    ///               }
    ///             },
    ///             "9": {
    ///               "layoutName": "1 Left, 2 Right",
    ///               "layoutIndex": 303,
    ///               "layoutType": "oneLeftTwoRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Left Window",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Right Top",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Right Bottom",
    ///                   "input": "input3"
    ///                 }
    ///               }
    ///             },
    ///             "10": {
    ///               "layoutName": "2 Top, 2 Bottom",
    ///               "layoutIndex": 401,
    ///               "layoutType": "twoTopTwoBottom",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Top Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Top Right",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Bottom Left",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Bottom Right",
    ///                   "input": "input4"
    ///                 }
    ///               }
    ///             },
    ///             "11": {
    ///               "layoutName": "1 Left, 3 Right",
    ///               "layoutIndex": 402,
    ///               "layoutType": "oneLeftThreeRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Left Large",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Right Top",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Right Middle",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Right Bottom",
    ///                   "input": "input4"
    ///                 }
    ///               }
    ///             },
    ///             "12": {
    ///               "layoutName": "1 Large Left, 4 Right",
    ///               "layoutIndex": 501,
    ///               "layoutType": "oneLargeLeftFourRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Large Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Small Right 1",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Small Right 2",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Small Right 3",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Small Right 4",
    ///                   "input": "input5"
    ///                 }
    ///               }
    ///             },
    ///             "13": {
    ///               "layoutName": "4 Left, 1 Large Right",
    ///               "layoutIndex": 502,
    ///               "layoutType": "fourLeftOneLargeRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Small Left 1",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Small Left 2",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Small Left 3",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Small Left 4",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Large Right",
    ///                   "input": "input5"
    ///                 }
    ///               }
    ///             },
    ///             "14": {
    ///               "layoutName": "2 Left, 1 Large Center, 2 Right",
    ///               "layoutIndex": 503,
    ///               "layoutType": "twoLeftOneLargeCenterTwoRight",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Left Top",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Left Bottom",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Large Center",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Right Top",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Right Bottom",
    ///                   "input": "input5"
    ///                 }
    ///               }
    ///             },
    ///             "15": {
    ///               "layoutName": "3 Top, 3 Bottom",
    ///               "layoutIndex": 601,
    ///               "layoutType": "threeTopThreeBottom",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Top Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Top Center",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Top Right",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Bottom Left",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Bottom Center",
    ///                   "input": "input5"
    ///                 },
    ///                 "6": {
    ///                   "label": "Bottom Right",
    ///                   "input": "input6"
    ///                 }
    ///               }
    ///             },
    ///             "16": {
    ///               "layoutName": "1 Large Left, 5 Stacked",
    ///               "layoutIndex": 602,
    ///               "layoutType": "oneLargeLeftFiveStacked",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Large Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Stack 1",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Stack 2",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Stack 3",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Stack 4",
    ///                   "input": "input5"
    ///                 },
    ///                 "6": {
    ///                   "label": "Stack 5",
    ///                   "input": "input6"
    ///                 }
    ///               }
    ///             },
    ///             "17": {
    ///               "layoutName": "5 Around, 1 Large Bottom Left",
    ///               "layoutIndex": 603,
    ///               "layoutType": "fiveAroundOneLargeBottomLeft",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Large Bottom Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Small Top Left",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Small Top Center",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Small Top Right",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Small Bottom Center",
    ///                   "input": "input5"
    ///                 },
    ///                 "6": {
    ///                   "label": "Small Bottom Right",
    ///                   "input": "input6"
    ///                 }
    ///               }
    ///             },
    ///             "18": {
    ///               "layoutName": "5 Around, 1 Large Top Left",
    ///               "layoutIndex": 604,
    ///               "layoutType": "fiveAroundOneLargeTopLeft",
    ///               "windows": {
    ///                 "1": {
    ///                   "label": "Large Top Left",
    ///                   "input": "input1"
    ///                 },
    ///                 "2": {
    ///                   "label": "Small Top Center",
    ///                   "input": "input2"
    ///                 },
    ///                 "3": {
    ///                   "label": "Small Top Right",
    ///                   "input": "input3"
    ///                 },
    ///                 "4": {
    ///                   "label": "Small Bottom Left",
    ///                   "input": "input4"
    ///                 },
    ///                 "5": {
    ///                   "label": "Small Bottom Center",
    ///                   "input": "input5"
    ///                 },
    ///                 "6": {
    ///                   "label": "Small Bottom Right",
    ///                   "input": "input6"
    ///                 }
    ///               }
    ///             }
    ///           }
    ///         }
    ///       }
    ///     }
    ///   }
    /// </summary>
    public class Nvx38xMultiviewConfig
    {
        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("screens")]
        public Dictionary<uint, ScreenInfo> Screens { get; set; }
    }
}
