using Capper;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class IDistributedCacheExtensions
    {
        public static async Task<T> ReadThroughAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func)
        {
            return (await ReadThroughWithResponseAsync(cache, key, func)).Value;
        }

        public static async Task<CacheResponse<TKey, T>> ReadThroughWithResponseAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, CancellationToken token = default(CancellationToken))
        {
            return await ReadThroughWithResponseAsync(cache, key, func, new DistributedCacheEntryOptions(), token);
        }

        public static async Task<CacheResponse<TKey, T>> ReadThroughWithResponseAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var responseType = CacheResponseType.Hit;

            var stopwatch = Stopwatch.StartNew();

            var keyString = key.ToString();

            var entry = await cache.GetAsync(keyString);

            T value;

            if (entry is null)
            {
                value = await func.Invoke();
                await cache.SetAsync(keyString, Serialize(value), options, token);
                responseType = CacheResponseType.Miss;
            }
            else
            {
                value = Deserialize<T>(entry);
            }

            stopwatch.Stop();

            return new CacheResponse<TKey, T>(key,value, responseType, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
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
