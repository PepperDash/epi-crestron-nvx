using System;
using PepperDash.Essentials.Core;

namespace NvxEpi.JoinMaps
{
    public class Nvx38xMultiviewJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("MultiviewEnter")]
        public JoinDataComplete MultiviewEnter = new JoinDataComplete(
            new JoinData { JoinNumber = 21, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enter - Select to update view, hold to always allow", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewEnabled")]
        public JoinDataComplete MultiviewEnable = new JoinDataComplete(
            new JoinData { JoinNumber = 22, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enable - Requires reboot, starts additional multicast stream", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewDisable")]
        public JoinDataComplete MultiviewDisable = new JoinDataComplete(
            new JoinData { JoinNumber = 23, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Disable - Requires reboot, stops multiview stream", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewLayout")]
        public JoinDataComplete MultiviewLayout = new JoinDataComplete(
            new JoinData { JoinNumber = 21, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Layout", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Analog });

        [JoinName("MultiviewAudioSource")]
        public JoinDataComplete MultiviewAudioSource = new JoinDataComplete(
            new JoinData { JoinNumber = 22, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Audio Source - Requires audio route set to 'multiview'", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Analog });

        // Stream URL joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1StreamUrl")]
        public JoinDataComplete MultiviewWindow1StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 21, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow2StreamUrl")]
        public JoinDataComplete MultiviewWindow2StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 22, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow3StreamUrl")]
        public JoinDataComplete MultiviewWindow3StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 23, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow4StreamUrl")]
        public JoinDataComplete MultiviewWindow4StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 24, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow5StreamUrl")]
        public JoinDataComplete MultiviewWindow5StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 25, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow6StreamUrl")]
        public JoinDataComplete MultiviewWindow6StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 26, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        // Label joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1Label")]
        public JoinDataComplete MultiviewWindow1Label = new JoinDataComplete(
            new JoinData { JoinNumber = 31, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow2Label")]
        public JoinDataComplete MultiviewWindow2Label = new JoinDataComplete(
            new JoinData { JoinNumber = 32, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow3Label")]
        public JoinDataComplete MultiviewWindow3Label = new JoinDataComplete(
            new JoinData { JoinNumber = 33, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow4Label")]
        public JoinDataComplete MultiviewWindow4Label = new JoinDataComplete(
            new JoinData { JoinNumber = 35, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow5Label")]
        public JoinDataComplete MultiviewWindow5Label = new JoinDataComplete(
            new JoinData { JoinNumber = 35, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow6Label")]
        public JoinDataComplete MultiviewWindow6Label = new JoinDataComplete(
            new JoinData { JoinNumber = 36, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        public Nvx38xMultiviewJoinMap(uint joinStart) : base(joinStart, typeof(Nvx38xMultiviewJoinMap)) { }
    }
}
