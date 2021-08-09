using Newtonsoft.Json;
using System.Text;

namespace Capper.Serialization
{
    public class JsonCacheSerializer : ICacheSerializer
    {
        public T Deserialize<T>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
        }

        public byte[] Serialize<T>(T deserialized)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deserialized));
        }
    }
}
