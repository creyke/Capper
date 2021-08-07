using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Capper
{
    public static class IDistributedCacheExtensions
    {
        public static async Task<T> ReadThroughAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func)
        {
            return (await ReadThroughWithResponseAsync(cache, key, func)).Value;
        }

        public static async Task<CacheResponse<T>> ReadThroughWithResponseAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func)
        {
            var responseType = CacheResponseType.Hit;

            var stopwatch = Stopwatch.StartNew();

            var keyString = key.ToString();

            var entry = await cache.GetAsync(keyString);

            T value;

            if (entry is null)
            {
                value = await func.Invoke();
                await cache.SetAsync(keyString, Serialize(value));
                responseType = CacheResponseType.Miss;
            }
            else
            {
                value = Deserialize<T>(entry);
            }

            stopwatch.Stop();

            return new CacheResponse<T>(value, responseType, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
        }

        private static T Deserialize<T>(byte[] serialized)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(serialized));
        }

        private static byte[] Serialize<T>(T deserialized)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deserialized));
        }
    }
}
