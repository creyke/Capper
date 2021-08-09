using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Capper.Samples.AspNet.Caches
{
    public interface IVehicleCache : IDistributedCache {}

    public class VehicleCache : MemoryDistributedCache, IVehicleCache
    {
        public VehicleCache(IOptions<MemoryDistributedCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }
    }
}
