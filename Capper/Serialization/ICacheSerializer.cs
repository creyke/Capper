namespace Capper.Serialization
{
    public interface ICacheSerializer
    {
        T Deserialize<T>(byte[] serialized);
        byte[] Serialize<T>(T deserialized);
    }
}
