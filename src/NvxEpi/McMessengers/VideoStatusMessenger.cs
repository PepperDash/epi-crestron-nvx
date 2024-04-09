using Newtonsoft.Json.Linq;
using NvxEpi.Devices;
using PepperDash.Essentials.AppServer.Messengers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NvxEpi.McMessengers
{
    public class VideoStatusMessenger:MessengerBase
    {
        private readonly NvxBaseDevice device;
        public VideoStatusMessenger(string key, string path, NvxBaseDevice device): base(key, path, device)
        {
            this.device = device;
        }

        protected override void RegisterActions()
        {
            base.RegisterActions();

            AddAction("/fullStatus", SendFullStatus);
        }

        private void SendFullStatus(string id, JToken content)
        {

        }
    }
}
