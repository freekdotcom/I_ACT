using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polenter.Serialization;

namespace ACD
{
    public class LogEvent : Event
    {
        public static readonly string TypeIdentifier = "debug:log";

        [ExcludeFromSerialization]
        public JObject Data { get; set; }

        [JsonIgnore]
        public string DataString
        {
            get { return Data?.ToString(); }
            set { Data = JObject.Parse(value); }
        }

        static readonly JsonSerializerSettings settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.None
        };

        public LogEvent()
        {
        }

        public LogEvent(object data)
            : base(DateTime.Now, TypeIdentifier)
        {
            DataString = JsonConvert.SerializeObject(data, Formatting.None, settings);
        }
    }
}

