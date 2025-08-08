using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using Crestron.SimplSharpPro.AudioDistribution;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Features.Audio;
using NvxEpi.Features.AutomaticRouting;
using NvxEpi.Features.Config;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Features.Streams.Usb;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;
using System.Collections.Generic;
using NvxEpi.Extensions;

namespace NvxEpi.Devices
    {
    public class DmNaxBtioDevice : CrestronGenericBaseDevice
        {
        private readonly DmNaxBluetooth _dmnaxbluetooth;
        private IBasicVolumeWithFeedback _audio;
        private readonly NvxDeviceProperties _config;
        private readonly DeviceConfig config;

        // Messenger events
        public event Action<eBluetoothStatus> BluetoothStatusChanged;
        public event Action<string> PairingCodeChanged;
        public event Action<bool> ConnectionChanged;
        public event Action<string> ConnectedDeviceNameChanged;
        public event Action<string> LocalDeviceNameChanged;

        public DmNaxBtioDevice(string ipId, string controlSystem, DeviceConfig deviceConfig)
            : base(ipId, controlSystem)
            {
            config = deviceConfig;
            _config = NvxDeviceProperties.FromDeviceConfig(config);

            // Robust IP ID parsing
            uint ipIdValue;
            if (ipId.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                if (!uint.TryParse(ipId.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out ipIdValue))
                    ipIdValue = 0; // fallback
                }
            else if (!uint.TryParse(ipId, out ipIdValue))
                {
                ipIdValue = 0; // fallback
                }

            // Robust control system check
            CrestronControlSystem cs = Global.ControlSystem;
            if (cs == null)
                throw new InvalidOperationException("CrestronControlSystem is not initialized.");

            // Wire up DmNaxBluetooth events and feedbacks if available
            if (_dmnaxbluetooth != null)
                {
                _dmnaxbluetooth.BluetoothPropertyChange += OnBluetoothPropertyChange;
                ConnectedDeviceNameFeedback = new StringFeedback(() =>
                    _dmnaxbluetooth.BluetoothDeviceNameFeedback != null
                        ? _dmnaxbluetooth.BluetoothDeviceNameFeedback.StringValue
                        : string.Empty);
                LocalDeviceNameFeedback = new StringFeedback(() =>
                    _dmnaxbluetooth.BluetoothDeviceName != null
                        ? _dmnaxbluetooth.BluetoothDeviceName.StringValue
                        : string.Empty);
                }
            }

        public ePairingCommand PairedDeviceCommand { get; set; }

        public StringOutputSig PairingCodeFeedback
            {
            get { return _dmnaxbluetooth != null ? _dmnaxbluetooth.PairingCodeFeedback : null; }
            }

        public StringFeedback ConnectedDeviceNameFeedback { get; private set; }
        public BoolFeedback IsConnectedFeedback { get; private set; }
        public StringFeedback LocalDeviceNameFeedback { get; private set; }

        public eBluetoothStatus BluetoothStatus
            {
            get { return _dmnaxbluetooth != null ? _dmnaxbluetooth.StatusFeedback : eBluetoothStatus.Disconnected; }
            }

        public BoolFeedback IsBluetoothConnected
            {
            get { return new BoolFeedback(() => BluetoothStatus == eBluetoothStatus.Connected); }
            }

        public void ExecutePairingCommand()
            {
            if (_dmnaxbluetooth != null)
                _dmnaxbluetooth.PairedDeviceCommand = PairedDeviceCommand;
            }

        public void ReleaseAllPairedDevices()
            {
            PairedDeviceCommand = ePairingCommand.ReleaseAllPairedDevices;
            ExecutePairingCommand();
            }

        public void ReleaseInactivePairedDevices()
            {
            PairedDeviceCommand = ePairingCommand.ReleaseInactivePairedDevices;
            ExecutePairingCommand();
            }

        public void ForgetAllPairedDevices()
            {
            PairedDeviceCommand = ePairingCommand.ForgetAllPairedDevices;
            ExecutePairingCommand();
            }

        public void ForgetInactivePairedDevices()
            {
            PairedDeviceCommand = ePairingCommand.ForgetInactivePairedDevices;
            ExecutePairingCommand();
            }

        public void ForgetConnectedDevice()
            {
            PairedDeviceCommand = ePairingCommand.ForgetConnectedDevice;
            ExecutePairingCommand();
            }

        // Messenger logic: handle hardware property changes
        private void OnBluetoothPropertyChange(object sender, EventArgs e)
            {
            BluetoothStatusChanged?.Invoke(BluetoothStatus);
            PairingCodeChanged?.Invoke(PairingCodeFeedback != null ? PairingCodeFeedback.StringValue : string.Empty);
            ConnectionChanged?.Invoke(BluetoothStatus == eBluetoothStatus.Connected);
            ConnectedDeviceNameChanged?.Invoke(ConnectedDeviceNameFeedback != null ? ConnectedDeviceNameFeedback.StringValue : string.Empty);
            LocalDeviceNameChanged?.Invoke(LocalDeviceNameFeedback != null ? LocalDeviceNameFeedback.StringValue : string.Empty);
            }

        // Optionally, call this to manually update messengers
        public void UpdateMessengers()
            {
            BluetoothStatusChanged?.Invoke(BluetoothStatus);
            PairingCodeChanged?.Invoke(PairingCodeFeedback != null ? PairingCodeFeedback.StringValue : string.Empty);
            ConnectionChanged?.Invoke(BluetoothStatus == eBluetoothStatus.Connected);
            ConnectedDeviceNameChanged?.Invoke(ConnectedDeviceNameFeedback != null ? ConnectedDeviceNameFeedback.StringValue : string.Empty);
            LocalDeviceNameChanged?.Invoke(LocalDeviceNameFeedback != null ? LocalDeviceNameFeedback.StringValue : string.Empty);
            }
        }
    }