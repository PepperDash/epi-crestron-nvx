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
    public static class NvxRouterBridge
    {
        public static void LinkToApiExt(this NvxRouterEpi router, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            var joinMap = new DmChassisControllerJoinMap();
            var joinMapSerialized = JoinMapHelper.GetJoinMapForDevice(joinMapKey);

            if (!string.IsNullOrEmpty(joinMapSerialized))
                joinMap = JsonConvert.DeserializeObject<DmChassisControllerJoinMap>(joinMapSerialized);


            joinMap.OffsetJoinNumbers(joinStart);

            Debug.Console(1, router, "Linking to Trilist '{0}'", trilist.ID.ToString("X"));

            for (uint x = 0; x < router.Config.NumberOfInputs; x++)
            {
                var inputNameJoin = x + joinMap.InputNames;
                var inputNameFb = router.GetInputNameFeedback(x + 1);
                LinkFeedbackAndFire(inputNameJoin, inputNameFb, trilist);

                var inputTxOnlineJoin = x + joinMap.InputEndpointOnline;
                var inputTxOnlineFb = router.GetTxOnlineFb(x + 1);
                LinkFeedbackAndFire(inputTxOnlineJoin, inputTxOnlineFb, trilist);

                var inputSyncDetectedJoin = x + joinMap.VideoSyncStatus;
                var inputSyncDetectedFb = router.GetHdmiSyncStatusFeedback(x + 1);
                LinkFeedbackAndFire(inputSyncDetectedJoin, inputSyncDetectedFb
            }

            for (uint x = 0; x < router.Config.NumberOfOutputs; x++)
            {
                var joinActual = x + joinMap.InputNames;
                var fb = router.GetOutputNameFeedback(x + 1);
                fb.LinkInputSig(trilist.StringInput[joinActual]);
                fb.FireUpdate();
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