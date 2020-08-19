using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace NvxEpi.DynRouting.Config
{
    public class DynNvxConfig
    {
        [JsonProperty("transmitters")]
        public Dictionary<string, string> Transmitters { get; set; }

        [JsonProperty("receivers")]
        public Dictionary<string, string> Receivers { get; set; }
    }
}