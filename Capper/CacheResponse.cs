using System;

namespace Capper
{
    public class CacheResponse<T>
    {
        public T Value { get; }
        public CacheResponseType ResponseType { get; }
        public TimeSpan ResponseTime { get; }

        public CacheResponse(T value, CacheResponseType responseType, TimeSpan responseTime)
        {
            Value = value;
            ResponseType = responseType;
            ResponseTime = responseTime;
        }
    }
}
