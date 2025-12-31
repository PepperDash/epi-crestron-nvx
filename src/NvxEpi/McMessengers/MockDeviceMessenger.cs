using Newtonsoft.Json;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;

namespace NvxEpi.McMessengers;

public class MockDeviceMessenger : MessengerBase
{
  private NvxMockDevice device;
  public MockDeviceMessenger(string key, string messagePath, NvxMockDevice device) : base(key, messagePath, device)
  {
    this.device = device;
  }

  protected override void RegisterActions()
  {
    AddAction("/fullStatus", (id, content) =>
    {
      SendFullStatus(id);
    });

    AddAction("/inputStatus", (id, content) =>
    {
      SendFullStatus(id);
    });

    device.SyncDetected.OutputChange += (o, a) =>
    {
      SendFullStatus();
    };
  }

  private void SendFullStatus(string id = null)
  {
    var message = new MockDeviceInputFullState
    {
      SyncDetected = device.Sync,
    };

    PostStatusMessage(message, id);
  }
}

public class MockDeviceInputFullState : DeviceStateMessageBase
{
    /// <summary>
    /// Whether or not sync is detected on any input
    /// </summary>
    [JsonProperty("syncDetected")]
    public bool SyncDetected { get; set; }    
}
