using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Capper.Samples.AspNet.Caches
{
    public interface IAnimalCache : IDistributedCache
    {
    }

    public class AnimalCache : MemoryDistributedCache, IAnimalCache
    {
        public AnimalCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}
