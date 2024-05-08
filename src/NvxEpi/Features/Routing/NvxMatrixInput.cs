using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Devices;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using System;
using System.Linq;

namespace NvxEpi.Features.Routing
{
    public class NvxMatrixInput : IRoutingInputSlot
    {
        private readonly NvxBaseDevice _device;

        public NvxMatrixInput(NvxBaseDevice device):base()
        {
            _device = device;

            if(_device is IHdmiInput hdmiInput)
            {
                foreach(var feedback in  hdmiInput.SyncDetected)
                {
                    feedback.Value.OutputChange += (o, a) => VideoSyncChanged?.Invoke(this, new EventArgs());
                }
            }
        }        

        public string TxDeviceKey => _device.Key;

        public int SlotNumber => _device.DeviceId;

        public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.SecondaryAudio;

        public string Name => _device.Name;

        public BoolFeedback IsOnline => _device.IsOnline;

        public bool VideoSyncDetected => _device is IHdmiInput inputDevice ? inputDevice.SyncDetected.Any(fb => fb.Value.BoolValue) : false;

        public string Key => $"{_device.Key}";

        public event EventHandler VideoSyncChanged;
    }
}
