using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using Polenter.Serialization;

namespace ACD
{
    /*
     * Class encapsulating serialization.
     * 
     * TODO: Add better support for Type objects and serialization of derived types (SerializationBinder)
     */
    public static class Serialization
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string Serialize(object o, bool indented = false)
        {
            return JsonConvert.SerializeObject(o, indented ? Formatting.Indented : Formatting.None, settings);
        }

        public static byte[] SerializeInternal(object o)
        {
            var s = new SharpSerializer(true);
            using (var m = new MemoryStream())
            {
                s.Serialize(o, m);
                return m.ToArray();
            }
        }

        public static T Deserialize<T>(string s)
        {
            return JsonConvert.DeserializeObject<T>(s, settings);
        }

        public static T Deserialize<T>(byte[] b)
        {
            var s = new SharpSerializer(true);
            using (var m = new MemoryStream(b))
            {
                return (T)s.Deserialize(m);
            }
        }

		public static T DeserializeInto<T>(string s, T obj)
		{
			JsonConvert.PopulateObject(s, obj);
			return obj;
		}
    }
}
