using System;

namespace Capper
{
    public class CacheResponse<TValue>
    {
        public TValue Value { get; }
        public CacheResponseType ResponseType { get; }
        public TimeSpan ResponseTime { get; }

        public CacheResponse(TValue value, CacheResponseType responseType, TimeSpan responseTime)
        {
            Value = value;
            ResponseType = responseType;
            ResponseTime = responseTime;
        }
    }
}
