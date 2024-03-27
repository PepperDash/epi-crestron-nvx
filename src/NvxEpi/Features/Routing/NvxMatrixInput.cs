using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Devices;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using System;
using System.Linq;

namespace NvxEpi.Features.Routing
{
    public class NvxMatrixInput : RoutingInputSlotBase
    {
        private readonly NvxBaseDevice _device;

        public NvxMatrixInput(NvxBaseDevice device):base()
        {
            _device = device;

            if(_device is IHdmiInput hdmiInput)
            {
                foreach(var feedback in  hdmiInput.Feedbacks)
                {
                    feedback.OutputChange += (o, a) => VideoSyncChanged?.Invoke(this, new EventArgs());
                }
            }
        }        

        public override string TxDeviceKey => _device.Key;

        public override int SlotNumber => _device.DeviceId;

        public override eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput | eRoutingSignalType.SecondaryAudio;

        public override string Name => _device.Name;

        public override BoolFeedback IsOnline => _device.IsOnline;

        public override bool VideoSyncDetected => _device is IHdmiInput inputDevice ? inputDevice.SyncDetected.Any(fb => fb.Value.BoolValue) : false;

        public override string Key => $"{_device.Key}-matrixInput";

        public override event EventHandler VideoSyncChanged;
    }
}
