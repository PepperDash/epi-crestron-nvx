using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using NvxEpi.Interfaces;

using PepperDash.Core;
using PepperDash.Essentials.Bridges;
using PepperDash.Essentials.Core;

using Newtonsoft.Json;

namespace NvxEpi
{
    public class NvxRouterBridge
    {
        private readonly Crestron.SimplSharpPro.DeviceSupport.BasicTriList _trilist;
        private readonly uint _joinStart;
        private readonly string _joinMapKey;
        private readonly IKeyed _router;

        public NvxRouterBridge(IKeyed router, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            _router = router;
            _trilist = trilist;
            _joinStart = joinStart;
            _joinMapKey = joinMapKey;
        }

        public void LinkToApi()
        {
            var router = _router as NvxRouterEpi;
            if (router == null) return;

            var joinMapKey = _joinMapKey;
            var joinStart = _joinStart;
            var trilist = _trilist;

            var joinMap = new DmChassisControllerJoinMap();
            var joinMapSerialized = JoinMapHelper.GetJoinMapForDevice(joinMapKey);

            if (!string.IsNullOrEmpty(joinMapSerialized))
                joinMap = JsonConvert.DeserializeObject<DmChassisControllerJoinMap>(joinMapSerialized);

            joinMap.OffsetJoinNumbers(joinStart);

            Debug.Console(1, router, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));

            for (uint x = 0; x < router.Config.NumberOfInputs; x++)
            {
                var currentInput = x + 1;

                var inputSyncDetectedJoin = currentInput + joinMap.VideoSyncStatus;
                var inputSyncDetectedFb = router.GetHdmiSyncStatusFeedback(currentInput);
                Debug.Console(2, router, "Linking 'Sync Detected' for input:{0} to Join {1}", currentInput, inputSyncDetectedJoin);
                LinkFeedbackAndFire(inputSyncDetectedJoin, inputSyncDetectedFb, trilist);

                var inputTxOnlineJoin = currentInput + joinMap.InputEndpointOnline;
                var inputTxOnlineFb = router.GetTxOnlineFb(currentInput);
                Debug.Console(2, router, "Linking 'Tx Online' for input:{0} to Join {1}", currentInput, inputTxOnlineJoin);
                LinkFeedbackAndFire(inputTxOnlineJoin, inputTxOnlineFb, trilist);

                var inputNameJoin = currentInput + joinMap.InputNames;
                var inputNameFb = router.GetInputNameFeedback(currentInput);
                Debug.Console(2, router, "Linking 'Name' for input:{0} to Join {1}", currentInput, inputNameJoin);
                LinkFeedbackAndFire(inputNameJoin, inputNameFb, trilist);

                var hdcpStateJoin = currentInput + joinMap.HdcpSupportState;
                var hdcpStateFb = router.GetHdcpStateFb(currentInput);
                Debug.Console(2, router, "Linking 'Hdcp State' for input:{0} to Join {1}", currentInput, hdcpStateJoin);
                LinkFeedbackAndFire(hdcpStateJoin, hdcpStateFb, trilist);

                trilist.SetUShortSigAction(hdcpStateJoin, state => router.SetHdcpState((int)currentInput, state));
            }

            for (uint x = 0; x < router.Config.NumberOfOutputs; x++)
            {
                var currentoutput = x + 1;

                var outputNameJoin = currentoutput + joinMap.OutputNames;
                var outputNameFb = router.GetOutputNameFeedback(currentoutput);
                Debug.Console(2, router, "Linking 'Name' for output:{0} to Join {1}", currentoutput, outputNameJoin);
                LinkFeedbackAndFire(outputNameJoin, outputNameFb, trilist);

                var outputTxOnlineJoin = currentoutput + joinMap.OutputEndpointOnline;
                var outputTxOnlineFb = router.GetTxOnlineFb(currentoutput);
                Debug.Console(2, router, "Linking 'Rx Online' for output:{0} to Join {1}", currentoutput, outputTxOnlineJoin);
                LinkFeedbackAndFire(outputTxOnlineJoin, outputTxOnlineFb, trilist);

                var outputVideoJoin = currentoutput + joinMap.OutputVideo;
                var outputVideoFb = router.GetVideoRouteFeedback(currentoutput);
                Debug.Console(2, router, "Linking 'Video Output' for output:{0} to Join {1}", currentoutput, outputVideoJoin);
                LinkFeedbackAndFire(outputVideoJoin, outputVideoFb, trilist);

                trilist.SetUShortSigAction(outputVideoJoin, source => router.RouteVideo(source, (int)currentoutput));

                var outputAudioJoin = currentoutput + joinMap.OutputAudio;
                var outputAudioFb = router.GetAudioRouteFeedback(currentoutput);
                Debug.Console(2, router, "Linking 'Audio Output' for output:{0} to Join {1}", currentoutput, outputAudioJoin);
                LinkFeedbackAndFire(outputAudioJoin, outputAudioFb, trilist);

                trilist.SetUShortSigAction(outputAudioJoin, source => router.RouteAudio(source, (int)currentoutput));

                var outputCurrentVideoNameJoin = currentoutput + joinMap.OutputCurrentVideoInputNames;
                var outputCurrentVideoNameFb = router.GetCurrentVideoRouteFeedback(currentoutput);
                Debug.Console(2, router, "Linking 'Current Video Input' for output:{0} to Join {1}", currentoutput, outputCurrentVideoNameJoin);
                LinkFeedbackAndFire(outputCurrentVideoNameJoin, outputCurrentVideoNameFb, trilist);

                var outputCurrentAudioNameJoin = currentoutput + joinMap.OutputCurrentAudioInputNames;
                var outputCurrentAudioNameFb = router.GetCurrentAudioRouteFeedback(currentoutput);
                Debug.Console(2, router, "Linking 'Current Audio Input' for output:{0} to Join {1}", currentoutput, outputCurrentAudioNameJoin);
                LinkFeedbackAndFire(outputCurrentAudioNameJoin, outputCurrentAudioNameFb, trilist);
            }
        }

        static void LinkFeedbackAndFire(uint joinNumber, StringFeedback feedback, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist)
        {
            feedback.LinkInputSig(trilist.StringInput[joinNumber]);
            feedback.FireUpdate();
        }

        static void LinkFeedbackAndFire(uint joinNumber, IntFeedback feedback, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist)
        {
            feedback.LinkInputSig(trilist.UShortInput[joinNumber]);
            feedback.FireUpdate();
        }

        static void LinkFeedbackAndFire(uint joinNumber, BoolFeedback feedback, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist)
        {
            feedback.LinkInputSig(trilist.BooleanInput[joinNumber]);
            feedback.FireUpdate();
        }
    }
}