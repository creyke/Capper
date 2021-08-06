using Buck.Samples.AspNet.Model;
using Buck.Samples.AspNet.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Buck.Samples.AspNet.Caches
{
    public class VehicleCache : ReadThroughCache<string, Vehicle[]>
    {
        private readonly VehicleRepository repository;

        public VehicleCache(VehicleRepository repository)
        {
            this.repository = repository;
        }

        protected async override Task<Vehicle[]> HydrateAsync(string key)
        {
            return (await repository.GetAsync(key)).ToArray();
        }
    }
}
