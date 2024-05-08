using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Features.Hdmi.Input;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using HdmiInput = NvxEpi.Features.Hdmi.Input.HdmiInput;

namespace NvxEpi.Devices
{
    public class NvxE3X : 
        NvxBaseDevice, 
        INvxE3XDeviceWithHardware, 
        IComPorts, 
        IIROutputPorts,
        IHdmiInput,
        IRouting
    {
        private IHdmiInput _hdmiInputs;
        private readonly IUsbStream _usbStream;

        public NvxE3X(DeviceConfig config, Func<DmNvxBaseClass> getHardware)
            : base(config, getHardware, true)
        {
            AddPreActivationAction(AddRoutingPorts);
        }

        public override bool CustomActivate()
        {
            var hardware = base.Hardware as DmNvxE3x;
            if (hardware == null)
                throw new Exception("hardware built doesn't match");

            Hardware = hardware;
            _hdmiInputs = new HdmiInput(this);

            AddMcMessengers();
            return base.CustomActivate();
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public new DmNvxE3x Hardware { get; private set; }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return _hdmiInputs.HdcpCapability; }
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

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return _hdmiInputs.SyncDetected; }
        }

        public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
        {
            get { return _hdmiInputs.CurrentResolution; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> AudioChannels {
            get
            {
                return _hdmiInputs.AudioChannels;
            }
        }

        public ReadOnlyDictionary<uint, StringFeedback> AudioFormat {
            get
            {
                return _hdmiInputs.AudioFormat;
            }
        }

        public ReadOnlyDictionary<uint, StringFeedback> ColorSpace {
            get
            {
                return _hdmiInputs.ColorSpace;
            }
        }

        public ReadOnlyDictionary<uint, StringFeedback> HdrType {
            get
            {
                return _hdmiInputs.HdrType;
            }
        }

        public ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString { get { return _hdmiInputs.HdcpCapabilityString; } }

        public ReadOnlyDictionary<uint, StringFeedback> HdcpSupport { get { return _hdmiInputs.HdcpSupport; } }

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
            HdmiInput1Port.AddRoutingPort(this);
            SwitcherForStreamOutput.AddRoutingPort(this);
            AnalogAudioInput.AddRoutingPort(this);
            SecondaryAudioInput.AddRoutingPort(this);
            SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
        }
    }
}