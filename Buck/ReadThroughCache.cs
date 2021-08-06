using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Buck
{
    public abstract class ReadThroughCache<TKey, TValue>
    {
        private readonly IDistributedCache cache;
        private readonly ICacheMetricService metrics;

        public ReadThroughCache(IDistributedCache cache, ICacheMetricService metrics)
        {
            this.cache = cache;
            this.metrics = metrics;
        }

        public ReadThroughCache(IDistributedCache cache) : this(cache, new DormantCacheMetricService())
        {
        }

        public ReadThroughCache() : this(new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())))
        {
        }

        public void Clear(TKey key)
        {
            cache.Remove(key.ToString());
        }

        public async Task<CacheResponse<TValue>> ReadAsync(TKey key)
        {
            var responseType = CacheResponseType.Hit;

            var stopwatch = Stopwatch.StartNew();

            var keyString = key.ToString();

            var entry = await cache.GetAsync(keyString);

            TValue value;

            if (entry is null)
            {
                value = await HydrateAsync(key);
                await cache.SetAsync(keyString, Serialize(value));
                responseType = CacheResponseType.Miss;
            }
            else
            {
                value = Deserialize<TValue>(entry);
            }

            stopwatch.Stop();

            return new CacheResponse<TValue>(value, responseType, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
        }

        private static T Deserialize<T>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
        }

        private static byte[] Serialize<T>(T deserialized)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deserialized));
        }

        protected abstract Task<TValue> HydrateAsync(TKey key);
    }
}
