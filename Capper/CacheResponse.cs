using System;

namespace Capper
{
    public class CacheResponse<TKey, T>
    {
        public TKey Key { get; }
        public T Value { get; }
        public CacheResponseType ResponseType { get; }
        public TimeSpan ResponseTime { get; }

        public CacheResponse(TKey key, T value, CacheResponseType responseType, TimeSpan responseTime)
        {
            Key = key;
            Value = value;
            ResponseType = responseType;
            ResponseTime = responseTime;
        }
    }
}
