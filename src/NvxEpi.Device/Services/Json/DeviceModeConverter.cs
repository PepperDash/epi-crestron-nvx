using System;
using Newtonsoft.Json;

namespace NvxEpi.Device.Services.Json
{
    public class DeviceModeConverter : JsonConverter
    {
        private const string _tx = "tx";
        private const string _rx = "rx";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var b = Convert.ToBoolean(value);
            writer.WriteValue(b ? _tx : _rx);
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(String);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;

            if (value == null || String.IsNullOrEmpty(value))
            {
                return false;
            }

            return value.Equals(_tx, StringComparison.OrdinalIgnoreCase);
        }
    }
}