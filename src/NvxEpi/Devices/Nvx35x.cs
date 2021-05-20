using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Features.Hdmi.Input;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices
{
    public class Nvx35X :
        NvxBaseDevice, 
        IComPorts, 
        IIROutputPorts,
        IUsbStream, 
        IHdmiInput, 
        IVideowallMode, 
        IRouting, 
        ICec, 
        INvx35XDeviceWithHardware
    {
        private IHdmiInput _hdmiInput;
        private IVideowallMode _hdmiOutput;
        private readonly IUsbStream _usbStream;

        public Nvx35X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
            : base(config, getHardware, isTransmitter)
        {
            AddPreActivationAction(AddRoutingPorts);
        }

        public override bool CustomActivate()
        {
            base.CustomActivate();

            var hardware = base.Hardware as DmNvx35x;
            if (hardware == null)
                throw new Exception("hardware built doesn't match");

            Hardware = hardware;

            _hdmiInput = new HdmiInput2(this);
            _hdmiOutput = new VideowallModeOutput(this);

            return true;
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _hdmiOutput.DisabledByHdcp; }
        }

        public new DmNvx35x Hardware { get; private set; }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return _hdmiInput.HdcpCapability; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _hdmiOutput.HorizontalResolution; }
        }

        public StringFeedback EdidManufacturer
        {
            get { return _hdmiOutput.EdidManufacturer; }
        }
        public IntFeedback VideoAspectRatioMode
        {
            get { return _hdmiOutput.VideoAspectRatioMode; }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
        }

        public bool IsRemote
        {
            get { return _usbStream.IsRemote; }
        }

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public Cec StreamCec
        {
            get { return Hardware.HdmiOut.StreamCec; }
        }

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return _hdmiInput.SyncDetected; }
        }

        public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
        {
            get { return _hdmiInput.CurrentResolution; }
        }

        public int UsbId
        {
            get { return _usbStream.UsbId; }
        }

        public StringFeedback UsbLocalId
        {
            get { return _usbStream.UsbLocalId; }
        }

        public StringFeedback UsbRemoteId
        {
            get { return _usbStream.UsbRemoteId; }
        }

        public IntFeedback VideowallMode
        {
            get { return _hdmiOutput.VideowallMode; }
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("outputSelector");

                Debug.Console(1,
                    this,
                    "Executing switch : '{0}' | '{1}' | '{2}'",
                    inputSelector.ToString(),
                    outputSelector.ToString(),
                    signalType.ToString());

                switcher.HandleSwitch(inputSelector, signalType);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error executing switch! : {0}", ex.Message);
            }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        }

        private void AddRoutingPorts()
        {
            SwitcherForHdmiOutput.AddRoutingPort(this);
            SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
            SwitcherForAnalogAudioOutput.AddRoutingPort(this);
            HdmiInput1Port.AddRoutingPort(this);
            HdmiInput2Port.AddRoutingPort(this);
            SecondaryAudioInput.AddRoutingPort(this);
            AnalogAudioInput.AddRoutingPort(this);

            if (IsTransmitter)
            {
                SwitcherForStreamOutput.AddRoutingPort(this);
            }
            else
            {
                StreamInput.AddRoutingPort(this);
            }
        }
    }
}