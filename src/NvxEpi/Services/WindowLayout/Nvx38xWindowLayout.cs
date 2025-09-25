using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using System;

namespace NvxEpi.Services.WindowLayout
{
    /// <summary>
    /// Extended Window Layout class specifically for DM-NVX-38x series devices
    /// Supports up to 6 video inputs and extended layout types beyond the standard WindowLayout
    /// </summary>
    public class Nvx38xWindowLayout
    {
        #region Enums

        /// <summary>
        /// Extended layout types for DM-NVX-38x series
        /// </summary>
        public enum ExtendedLayoutType
        {
            /// <summary>Full Screen</summary>
            Fullscreen = 0,
            /// <summary>Side by Side</summary>
            SideBySide = 201,
            /// <summary>PIP Small Top Left</summary>
            PipSmallTopLeft = 202,
            /// <summary>PIP Small Top Right</summary>
            PipSmallTopRight = 203,
            /// <summary>PIP Small Bottom Left</summary>
            PipSmallBottomLeft = 204,
            /// <summary>PIP Small Bottom Right</summary>
            PipSmallBottomRight = 205,
            /// <summary>1 Top, 2 Bottom</summary>
            OneTopTwoBottom = 301,
            /// <summary>2 Top, 1 Bottom</summary>
            TwoTopOneBottom = 302,
            /// <summary>1 Left, 2 Right</summary>
            OneLeftTwoRight = 303,
            /// <summary>2 Top, 2 Bottom</summary>
            TwoTopTwoBottom = 401,
            /// <summary>1 Left, 3 Right</summary>
            OneLeftThreeRight = 402,
            /// <summary>1 Large Left, 4 Right</summary>
            OneLargeLeftFourRight = 501,
            /// <summary>4 Left, 1 Large Right</summary>
            FourLeftOneLargeRight = 502,
            /// <summary>4 Left, 1 Large Right</summary>
            TwoLeftOneLargeCenterTwoRight = 503,
            /// <summary>3 Top, 3 Bottom</summary>
            ThreeTopThreeBottom = 601,
            /// <summary>1 Large Left, 5 Stacked</summary>
            OneLargeLeftFiveStacked = 602,
            /// <summary>5 Around, 1 Large Bottom Left</summary>
            FiveAroundOneLargeBottomLeft = 603,
            /// <summary>5 Around, 1 Large Top Left</summary>
            FiveAroundOneLargeTopLeft = 604
        }

        /// <summary>
        /// Extended video source types for DM-NVX-38x
        /// </summary>
        public enum ExtendedVideoSourceType
        {
            /// <summary>None</summary>
            None = 0
        }

        #endregion

        #region Fields

        private readonly DmNvx38x _device;
        private readonly IKeyed _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Video sources for each window (1-6)
        /// </summary>
        public ExtendedVideoSourceType[] WindowVideoSources { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for Nvx38xWindowLayout
        /// </summary>
        /// <param name="device">DM-NVX-38x device</param>
        /// <param name="parent">Parent device for logging</param>
        public Nvx38xWindowLayout(DmNvx38x device, IKeyed parent)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            WindowVideoSources = new ExtendedVideoSourceType[1];
        }

        #endregion
    }

    #region Event Args Classes

    /// <summary>
    /// Event arguments for layout change events
    /// </summary>
    public class LayoutChangeEventArgs : EventArgs
    {
        public Nvx38xWindowLayout.ExtendedLayoutType NewLayout { get; }

        public LayoutChangeEventArgs(Nvx38xWindowLayout.ExtendedLayoutType newLayout)
        {
            NewLayout = newLayout;
        }
    }

    /// <summary>
    /// Event arguments for window source change events
    /// </summary>
    public class WindowSourceChangeEventArgs : EventArgs
    {
        public uint WindowId { get; }
        public Nvx38xWindowLayout.ExtendedVideoSourceType NewSource { get; }

        public WindowSourceChangeEventArgs(uint windowId, Nvx38xWindowLayout.ExtendedVideoSourceType newSource)
        {
            WindowId = windowId;
            NewSource = newSource;
        }
    }

    #endregion
}