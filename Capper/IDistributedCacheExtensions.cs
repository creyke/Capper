using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capper
{
    public static class IDistributedCacheExtensions
    {
        public static async Task<IEnumerable<T>> ReadThroughAsync<TKey, T>(this IDistributedCache cache, TKey key, Func<Task<IEnumerable<T>>> func)
        {
            return await func.Invoke();
        }
    }
}
