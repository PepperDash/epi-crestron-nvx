using System.Linq;
using Newtonsoft.Json.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Features.Routing;
using PepperDash.Core.Logging;
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core;

namespace NvxEpi.McMessengers;
/*
public class MatrixRouterMessenger : MessengerBase
{
    private readonly NvxGlobalRouter device;
    public MatrixRouterMessenger(string key, string messagePath, NvxGlobalRouter device) : base(key, messagePath, device)
    {
        this.device = device;

        foreach (var slot in device.InputSlots.Values)
        {
            if (slot is NvxMatrixInput inputSlot)
            {
                var tx = DeviceManager.GetDeviceForKey<INvxDevice>(inputSlot.TxDeviceKey);
                tx.EnabledFeedback.OutputChange += (o, args) => SendUpdate();
            }
        }

        foreach (var slot in device.OutputSlots.Values)
        {
            if (slot is NvxMatrixOutput outputSlot)
            {
                var rx = DeviceManager.GetDeviceForKey<INvxDevice>(outputSlot.RxDeviceKey);
                rx.EnabledFeedback.OutputChange += (o, args) => SendUpdate();
            }
        }
    }

    public void SendUpdate()
    {
        var inputs = device.InputSlots.Where(kvp => kvp.Value is NvxMatrixInput input && input.IsEnabled)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => new RoutingInput(kvp.Value));

        var outputs = device.OutputSlots.Where(kvp => kvp.Value is NvxMatrixOutput output && output.IsEnabled)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => new RoutingOutput(kvp.Value));

        var message = new MatrixStateMessage
        {
            Inputs = inputs,
            Outputs = outputs
        };

        this.LogInformation("Sending matrix state update message: {message} path {path}", message, MessagePath);

        PostStatusMessage(JToken.FromObject(message));
    }
}*/
