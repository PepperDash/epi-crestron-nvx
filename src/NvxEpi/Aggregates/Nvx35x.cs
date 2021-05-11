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
using NvxEpi.Entities.Config;
using NvxEpi.Entities.Hdmi.Input;
using NvxEpi.Entities.Hdmi.Output;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Aggregates
{
    public class Nvx35X : NvxBaseDevice, IComPorts, IIROutputPorts,
        IUsbStream, IHdmiInput, IVideowallMode, IRouting, ICec, INvx35XDeviceWithHardware
    {
        private readonly DmNvx35x _hardware;
        private readonly IHdmiInput _hdmiInput;
        private readonly IVideowallMode _hdmiOutput;
        private readonly IUsbStream _usbStream;

        public Nvx35X(DeviceConfig config, DmNvx35x hardware)
            : base(config, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            _hardware = hardware;

            _hdmiInput = new HdmiInput2(this);
            _hdmiOutput = new VideowallModeOutput(this);

            RegisterForOnlineFeedback(hardware, props);
            RegisterForDeviceFeedback();
            AddRoutingPorts();
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _hdmiOutput.DisabledByHdcp; }
        }

        public new DmNvx35x Hardware
        {
            get { return _hardware; }
        }

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
            HdmiInput1Port.AddRoutingPort(this);
            HdmiInput2Port.AddRoutingPort(this);
            SwitcherForHdmiOutput.AddRoutingPort(this);

            if (IsTransmitter)
            {
                SwitcherForStreamOutput.AddRoutingPort(this);
                SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
                AnalogAudioInput.AddRoutingPort(this);
            }
            else
            {
                StreamInput.AddRoutingPort(this);
                SecondaryAudioInput.AddRoutingPort(this);
                SwitcherForAnalogAudioOutput.AddRoutingPort(this);
            }
        }

        private void RegisterForDeviceFeedback()
        {
            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
                {
                    if (IsTransmitter)
                        Hardware.SetTxDefaults(props);
                    else
                        Hardware.SetRxDefaults(props);
                };
        }
    }
}