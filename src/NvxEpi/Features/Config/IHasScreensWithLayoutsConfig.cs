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
    ///               "layoutName": "Name Single",
    ///               "layoutIndex": 1,
    ///               "layoutType": "Single",
    ///                 "windows": {
    ///                   "1": {
    ///                     "label": "Full Screen",
    ///                     "input": "input1"
    ///                   }
    ///                 }
    ///             },
    ///             "2": {
    ///               "layoutName": "Name Dual",
    ///               "layoutIndex": 2,
    ///               "layoutType": "Dual",
    ///                 "windows": {
    ///                   "1": {
    ///                     "label": "Room A Audience",
    ///                     "input": "input1"
    ///                   },
    ///                   "2": {
    ///                     "label": "Room B Audience",
    ///                     "input": "input2"
    ///                   }
    ///                 }
    ///             },
    ///             "3": {
    ///               "layoutName": "Name Triple",
    ///               "layoutIndex": 4,
    ///               "layoutType": "Triple",
    ///                 "windows": {
    ///                   "1": {
    ///                     "label": "Room A Audience",
    ///                     "input": "input1"
    ///                   },
    ///                   "2": {
    ///                     "label": "Room B Audience",
    ///                     "input": "input2"
    ///                   },
    ///                   "3": {
    ///                     "label": "Room B Presenter",
    ///                     "input": "input3"
    ///                   }
    ///                 }
    ///             },
    ///             "4": {
    ///               "layoutName": "Name Quad",
    ///               "layoutIndex": 5,
    ///               "layoutType": "Quad",
    ///                 "windows": {
    ///                   "1": {
    ///                     "label": "Room A Audience",
    ///                     "input": "input1"
    ///                   },
    ///                   "2": {
    ///                     "label": "Room B Audience",
    ///                     "input": "input2"
    ///                   },
    ///                   "3": {
    ///                     "label": "Room B Presenter",
    ///                     "input": "input3"
    ///                   },
    ///                   "4": {
    ///                     "label": "Room C Audience",
    ///                     "input": "input4"
    ///                   }
    ///                 }
    ///             }
    ///           }
    ///         }
    ///       }
    ///     }
    ///   },
    /// </summary>
    public class IHasScreensWithLayoutsConfig
    {
        [JsonProperty("control")]
        public ControlPropertiesConfig Control { get; set; }

        [JsonProperty("screens")]
        public Dictionary<uint, ScreenInfo> Screens { get; set; }
    }
}
