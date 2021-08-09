using Capper;
using Capper.Serialization;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class IDistributedCacheExtensions
    {
        private static readonly ICacheSerializer Serializer = new JsonCacheSerializer();

        public static async Task<T> ReadThroughAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, ICacheSerializer serializer = null, CancellationToken token = default(CancellationToken))
        {
            return (await ReadThroughWithResponseAsync(cache, key, func, serializer, token)).Value;
        }

        public static async Task<T> ReadThroughAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, DistributedCacheEntryOptions options, ICacheSerializer serializer = null, CancellationToken token = default(CancellationToken))
        {
            return (await ReadThroughWithResponseAsync(cache, key, func, options, serializer, token)).Value;
        }

        public static async Task<CacheResponse<TKey, T>> ReadThroughWithResponseAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, ICacheSerializer serializer = null, CancellationToken token = default(CancellationToken))
        {
            return await ReadThroughWithResponseAsync(cache, key, func, new DistributedCacheEntryOptions(), serializer, token);
        }

        public static async Task<CacheResponse<TKey, T>> ReadThroughWithResponseAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<T>> func, DistributedCacheEntryOptions options, ICacheSerializer serializer = null, CancellationToken token = default(CancellationToken))
        {
            var responseType = CacheResponseType.Hit;

            var stopwatch = Stopwatch.StartNew();

            var keyString = key.ToString();

            var entry = await cache.GetAsync(keyString);

            T value;

            serializer ??= Serializer;

            if (entry is null)
            {
                value = await func.Invoke();
                await cache.SetAsync(keyString, serializer.Serialize(value), options, token);
                responseType = CacheResponseType.Miss;
            }
            else
            {
                value = serializer.Deserialize<T>(entry);
            }

            stopwatch.Stop();

            return new CacheResponse<TKey, T>(key,value, responseType, TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds));
        }
    }
}
