using Capper.Samples.AspNet.Model;
using Capper.Samples.AspNet.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Capper.Samples.AspNet.Caches
{
    public class VehicleCache : Cache<string, Vehicle[]>
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
