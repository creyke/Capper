using Buck.Samples.AspNet.Model;
using Buck.Samples.AspNet.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Buck.Samples.AspNet.Caches
{
    public class AnimalCache : ReadThroughCache<string, Animal[]>
    {
        private readonly AnimalRepository repository;

        public AnimalCache(AnimalRepository repository)
        {
            this.repository = repository;
        }

        protected async override Task<Animal[]> HydrateAsync(string key)
        {
            return (await repository.GetAsync(key)).ToArray();
        }
    }
}
