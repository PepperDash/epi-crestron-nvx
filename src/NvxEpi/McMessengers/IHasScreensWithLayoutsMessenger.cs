using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.AppServer.Messengers;
using System.Collections.Generic;
using System.Linq;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using IHasScreensWithLayouts = PepperDash.Essentials.Core.DeviceTypeInterfaces.IHasScreensWithLayouts;

namespace NvxEpi.McMessengers;

public class IHasScreensWithLayoutsMessenger : MessengerBase
{
    private IHasScreensWithLayouts _hasScreensWithLayouts;

    public IHasScreensWithLayoutsMessenger(string key, string messagePath, IHasScreensWithLayouts hasScreensWithLayouts)
        : base(key, messagePath, hasScreensWithLayouts as IKeyName)
        {
            _hasScreensWithLayouts = hasScreensWithLayouts;
        }

    protected override void RegisterActions()
    {
        base.RegisterActions();

        AddAction("/fullStatus", (id, context) => SendFullStatus());
    }

    private void SendFullStatus()
        {
        var state = new IHasScreensWithLayoutsStateMessage
            {
            Screens = _hasScreensWithLayouts.Screens
            };
        PostStatusMessage(state);
        }

    ///send current layout info  
    public void SendCurrentLayoutStatus(uint screenId, LayoutInfo layout)
        {
        var data = new CurrentLayoutStatusMessage
            {
            ScreenId = screenId,
            LayoutIndex = layout.LayoutIndex,
            LayoutName = layout.LayoutName,
            LayoutType = layout.LayoutType,
            Windows = layout.Windows.ToDictionary(
                w => w.Key.ToString(),
                w => new WindowInfo { Label = w.Value.Label, Input = w.Value.Input }
            )
            };

        PostStatusMessage(data);
        }
}

public class IHasScreensWithLayoutsStateMessage : DeviceStateMessageBase
{
    [JsonProperty("screens")]
    public Dictionary<uint, ScreenInfo> Screens { get; set; }
}

public class CurrentLayoutStatusMessage : DeviceStateMessageBase
{
    [JsonProperty("screenId")]
    public uint ScreenId { get; set; }

    [JsonProperty("layoutIndex")]
    public int LayoutIndex { get; set; }

    [JsonProperty("layoutName")]
    public string LayoutName { get; set; }

    [JsonProperty("layoutType")]
    public string LayoutType { get; set; }

    [JsonProperty("windows")]
    public Dictionary<string, WindowInfo> Windows { get; set; }
}

public class WindowInfo
{
    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("input")]
    public string Input { get; set; }
}


